using System.Security.Claims;
using AutoMapper;
using BlogApp.Core.Data;
using BlogApp.Core.Data.Repositories;
using BlogApp.Core.Domain;
using BlogApp.Core.Services;
using BlogApp.Models;
using BlogApp.ViewModel;
using BlogApp.ViewModels;
using BlogApp.Web.Helper;
using BlogApp.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogApp.Controllers
{
    [Route("[Controller]")]
    public class PostController : Controller
    {
        #region Private Fields

        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;
        private readonly ICategoryService _categoryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        #endregion

        #region Contructor

        public PostController(
            IPostService postService,
            ICommentService commentService,
            ILikeService likeService,
            IUnitOfWork unitOfWork,
            ICategoryService categoryService,
            IMapper mapper)
        {
            _postService = postService;
            _commentService = commentService;
            _likeService = likeService;
            _unitOfWork = unitOfWork;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        [HttpGet("")]
        public async Task<IActionResult> Index(
         int? categoryId,
           bool is_most_Liked = false,
           PostSortOrderEnum sortOrder = PostSortOrderEnum.Newest,
           int page = 1)
        {
        
            int pageSize = 6;

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            bool isAdmin = User.IsInRole("Admin");

            var (posts, totalCount) = await _postService.GetPostsAsync(
                categoryId,
                is_most_Liked,
                sortOrder,
                page,
                pageSize,
                userId,
                isAdmin);

            var postsListViewModel = new PostsListViewModel
            {
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                SortOrder = sortOrder,
                IsMostLiked = is_most_Liked,
                CategoryId = categoryId,
                Categories = _mapper.Map<List<CategoryViewModel>>(
                    _categoryService.GetAllCategories().ToList()),
                Posts = _mapper.Map<List<PostViewModel>>(posts)
            };

            return View(postsListViewModel);
        }

        [HttpGet("Detail")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0)
                return NotFound();

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool isAdmin = User.IsInRole("Admin");

            var post = await _postService.GetPostDetailAsync(id);

            var comments = await _commentService.GetCommnetsByPostIdAsync(id);
            await _unitOfWork.SaveAsync();
            var commentViewModel = comments.Select(c => new CommentViewModel
            {
                Id = c.Id,
                Content = c.Content,
                CommentDate = c.CommentDate,
                UserName = c.User?.UserName ?? "",
                PostId = c.PostId,
                CanDelete = isAdmin || c.UserId == userId || c.Post.CreatedByUserId == userId
            }).ToList();

            var postVM = _mapper.Map<PostViewModel>(post);
            postVM.Comments = commentViewModel;

            return View(postVM);
        }


        [HttpGet("Create")]
        [Authorize(Roles = "Admin,Author")]
        public IActionResult Create()
        {
            var vm = new PostViewModel();
            vm.Categories = LoadCategories();
            return View(vm);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> Create(PostViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = LoadCategories();
                return View(vm);
            }

            try
            {


                var post = _mapper.Map<Post>(vm);
                if (vm.FeatureImage != null)
                {
                    post.FeatureImagePath = ImagesHelper.UploadFile(vm.FeatureImage);
                }
                await _postService.CreatePostAsync(post);
                await _unitOfWork.SaveAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                vm.Categories = LoadCategories();
                return View(vm);
            }
        }
        [Authorize(Roles = "Admin,Author")]
        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postService.GetPostDetailAsync(id);

            if (post == null)
                return NotFound();

            var vm = _mapper.Map<PostViewModel>(post);
            vm.Categories = LoadCategories();

            return View(vm);
        }

        [HttpPost("Edit")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> Edit(PostViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = LoadCategories();
                return View(vm);
            }

            var post = await _postService.GetPostDetailAsync(vm.Id);

            if (post == null)
                return NotFound();


            if (vm.FeatureImage != null && vm.FeatureImage.Length > 0)
            {

                ImagesHelper.DeleteFile(post.FeatureImagePath);

                post.FeatureImagePath = ImagesHelper.UploadFile(vm.FeatureImage);
            }

            post.Title = vm.Title;
            post.Content = vm.Content;
            post.CategoryId = vm.CategoryId;

            await _postService.EditPostAsync(post);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _postService.GetPostDetailAsync(id);

            if (post == null)
                return NotFound();

            return View(post);
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            try
            {
                var post = await _postService.GetPostDetailAsync(id);
                if (post == null)
                    return NotFound();

                await _postService.DeletePostAsync(id);

                await _unitOfWork.SaveAsync();
                ImagesHelper.DeleteFile(post.FeatureImagePath);

                TempData["SuccessMessage"] = "Post deleted successfully!";
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return RedirectToAction("Index");
        }

        [HttpPost("AddComment")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] CommentViewModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin");


            var comment = _mapper.Map<Comment>(model);
            comment.UserId = userId;

            await _commentService.AddCommentAsync(comment);
            await _unitOfWork.SaveAsync();

            var comments = await _commentService.GetCommnetsByPostIdAsync(model.PostId);


            var commentVM = _mapper.Map<List<CommentViewModel>>(comments);

            foreach (var c in commentVM)
            {
                var original = comments.First(x => x.Id == c.Id);

                c.CanDelete = isAdmin
                    || original.UserId == userId
                    || original.Post.CreatedByUserId == userId;
            }

            return PartialView("PostCommentsList", commentVM);
        }
        [HttpPost("DeleteComment")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id, int postId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin");

            var deleted = await _commentService.DeleteCommentAsync(id, userId, isAdmin);
            await _unitOfWork.SaveAsync();

            if (!deleted)
                return Forbid();

            var comments = await _commentService.GetCommnetsByPostIdAsync(postId);


            var commentVM = _mapper.Map<List<CommentViewModel>>(comments);

            foreach (var c in commentVM)
            {
                var original = comments.First(x => x.Id == c.Id);

                c.CanDelete = isAdmin
                    || original.UserId == userId
                    || original.Post.CreatedByUserId == userId;
            }

            return PartialView("PostCommentsList", commentVM);
        }

        [HttpPost("ToggleLike")]
        [Authorize]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            string? userIdNullable = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdNullable))
            {

                return Forbid();
            }

            string userId = userIdNullable;

            var liked = await _likeService.ToggleLikeAsync(postId, userId);
            await _unitOfWork.SaveAsync();

            var count = await _likeService.GetLikesCountByPostId(postId);

            return Json(new
            {
                liked = liked,
                count = count
            });
        }

        [Authorize(Roles = "Admin,Author")]
        [HttpPut("TogglePublish")]
        public async Task<IActionResult> TogglePublish(int id)
        {
            try
            {

                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


                if (string.IsNullOrEmpty(userId))
                {
                    return Forbid();
                }

                bool isAdmin = User.IsInRole("Admin");

                await _postService.TogglePublishAsync(id, userId, isAdmin);
                await _unitOfWork.SaveAsync();
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        #endregion

        #region Private Methods

        private IEnumerable<SelectListItem> LoadCategories()
        {
            return _categoryService.GetAllCategories()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
        }

        #endregion
    }
}