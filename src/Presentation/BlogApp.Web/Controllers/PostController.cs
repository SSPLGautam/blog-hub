using AutoMapper;
using BlogApp.Core.Domain;
using BlogApp.Shared.Dto;
using BlogApp.ViewModels;
using BlogApp.Web.Services;
using BlogApp.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogApp.Controllers
{
    [Route("[Controller]")]
    public class PostController : Controller
    {
        #region Private Fields

        private readonly PostApiService _apiService;
        private readonly CommentApiService _commentApi;
        private readonly LikeApiService _likeApi;
        private readonly CategoryApiService _categoryApi;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public PostController(
            PostApiService apiService,
            CommentApiService commentApi,
            LikeApiService likeApi,
            CategoryApiService categoryApi,
            IMapper mapper)
        {
            _apiService = apiService;
            _commentApi = commentApi;
            _likeApi = likeApi;
            _categoryApi = categoryApi;
            _mapper = mapper;
        }

        #endregion

        #region INDEX

        [HttpGet("")]
        public async Task<IActionResult> Index(
            int? categoryId,
            bool is_most_Liked = false,
            PostSortOrderEnum sortOrder = PostSortOrderEnum.Newest,
            int page = 1)
        {
            int pageSize = 6;
     
            (List<PostResponceDto> postsDto, int totalCount) = await _apiService.GetPostsAsync(
                categoryId,
                is_most_Liked,
                sortOrder.ToString(),
                page,
                pageSize);

            var categoriesDto = await _categoryApi.GetCategories();

            var vm = new PostsListViewModel
            {
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                SortOrder = sortOrder,
                IsMostLiked = is_most_Liked,
                CategoryId = categoryId,
                Categories = _mapper.Map<List<CategoryViewModel>>(categoriesDto),
                Posts = _mapper.Map<List<PostViewModel>>(postsDto)
            };

            return View(vm);
        }
                      
        #endregion

        #region DETAIL

        [HttpGet("Detail")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0)
                return NotFound();

            var postDto = await _apiService.GetPostAsync(id);
            var commentsDto = await _commentApi.GetComments(id);

            var vm = _mapper.Map<PostViewModel>(postDto);
            vm.Comments = _mapper.Map<List<CommentViewModel>>(commentsDto);

            return View(vm);
        }

        #endregion

        #region CREATE

        [HttpGet("Create")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> Create()
        {
            var vm = new PostViewModel
            {
                Categories = await LoadCategories()
            };

            return View(vm);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> Create(PostViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await LoadCategories();
                return View(vm);
            }

            var dto = _mapper.Map<PostCreatedDto>(vm);

            await _apiService.CreatePostAsync(dto);

            return RedirectToAction("Index");
        }

        #endregion

        #region EDIT

        [HttpGet("Edit")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> Edit(int id)
        {
            var postDto = await _apiService.GetPostAsync(id);

            if (postDto == null)
                return NotFound();

            var vm = _mapper.Map<PostViewModel>(postDto);
            vm.Categories = await LoadCategories();

            return View(vm);
        }
                         
        [HttpPost("Edit")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> Edit(PostViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await LoadCategories();
                return View(vm);
            }

            var dto = _mapper.Map<PostCreatedDto>(vm);

            await _apiService.UpdatePostAsync(dto);

            return RedirectToAction("Index");
        }      

        #endregion

        #region DELETE

        [HttpGet("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var postDto = await _apiService.GetPostAsync(id);

            if (postDto == null)
                return NotFound();

            var vm = _mapper.Map<PostViewModel>(postDto);

            return View(vm);
        }
                                  
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            await _apiService.DeletePostAsync(id);
            return RedirectToAction("Index");
        }

        #endregion

        #region COMMENT

        [HttpPost("AddComment")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] CommentViewModel model)
        {

            var dto = _mapper.Map<CommentCreateDto>(model);

            await _commentApi.AddComment(dto);

            var commentsDto = await _commentApi.GetComments(model.PostId);

            var commentVM = _mapper.Map<List<CommentViewModel>>(commentsDto);

            return PartialView("PostCommentsList", commentVM);
        }

        [HttpPost("DeleteComment")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id, int postId)
        {
            await _commentApi.DeleteComment(id);

            var commentsDto = await _commentApi.GetComments(postId);

            var commentVM = _mapper.Map<List<CommentViewModel>>(commentsDto);

            return PartialView("PostCommentsList", commentVM);
        }

        #endregion

        #region LIKE
  
        [HttpPost("ToggleLike")]
     
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var result = await _likeApi.ToggleLike(postId);

            if (result == null)
                return Unauthorized(); // or: return Json(new { liked = false, count = 0 });

            return Json(new
            {
                liked = result.Liked,
                count = result.Count
            });
        }

        #endregion

        #region PRIVATE

        private async Task<IEnumerable<SelectListItem>> LoadCategories()
        {
            var categories = await _categoryApi.GetCategories();

            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
        }

        #endregion
    }
}