using System.Security.Claims;
using BlogApp.Models;
using BlogApp.Repository;
using BlogApp.Service;
using BlogApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogApp.Controllers
{
    [Route("[Controller]")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;
        private readonly IGenericRepository<Category> _categoryRepository;

        public PostController(
            IPostService postService,
            ICommentService commentService,
            ILikeService likeService,
            IGenericRepository<Category> categoryRepository)
        {
            _postService = postService;
            _commentService = commentService;
            _likeService = likeService;
            _categoryRepository = categoryRepository;
        }

  
        [HttpGet("")]
        public async Task<IActionResult> Index(
            int? categoryId,
            bool is_most_Liked = false,
            string sortOrder = "newest",
            int page = 1)
        {
            int pageSize = 6;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            bool isAdmin = User.IsInRole("Admin");

            var result = await _postService.GetPostsAsync(
                categoryId,
                is_most_Liked,
                sortOrder,
                page,
                pageSize,
                userId,
                isAdmin);

            ViewBag.Categories = _categoryRepository.GetAll().ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.totalCount / pageSize);
            ViewBag.CategoryId = categoryId;
            ViewBag.SortOrder = sortOrder;
            ViewBag.IsMostLiked = is_most_Liked;

            return View(result.posts);
        }

    
        [HttpGet("Detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var post = await _postService.GetPostDetailAsync(id);

            if (post == null)
                return NotFound();

            return View(post);
        }

        [HttpGet("Create")]
        [Authorize(Roles = "Admin,Author")]
        public IActionResult Create()
        {
            var vm = new CreatePostViewModel();
            LoadCategories(vm);
            return View(vm);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> Create(CreatePostViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                LoadCategories(vm);
                return View(vm);
            }

            try
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                   
                    return Forbid();
                }

                await _postService.CreatePostAsync(vm, userId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                LoadCategories(vm);
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

            return View(new EditPostViewModel { post = post });
        }

        [Authorize(Roles = "Admin,Author")]
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(EditPostViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                 if (userId == null)
                {
                    
                    return Forbid();
                }

                bool isAdmin = User.IsInRole("Admin");

                await _postService.EditPostAsync(vm, userId, isAdmin);
                return RedirectToAction("Index");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
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
                await _postService.DeletePostAsync(id);
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
        public async Task<IActionResult> AddComment([FromBody] Comment comment)
        {
        
            string? userName = User.Identity?.Name;

            
            if (string.IsNullOrWhiteSpace(userName))
            {
              
                return Forbid();
            }

         
            await _commentService.AddCommentAsync(comment, userName!);

         
            return Json(new
            {
                username = comment.UserName,
                content = comment.Content,
                commentDate = comment.CommentDate.ToString("dd MMM yyyy")
            });
        }

        [HttpPost("DeleteComment")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            await _commentService.DeleteCommentAsync(id);
            return Ok();
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

            var result = await _likeService.ToggleLikeAsync(postId, userId);

            return Json(new
            {
                liked = result.liked,
                count = result.count
            });
        }


        [Authorize(Roles = "Admin,Author")]
        [HttpPost("TogglePublish")]
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
                return RedirectToAction("Detail", new { id });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

   
        private void LoadCategories(CreatePostViewModel vm)
        {
            var categories = _categoryRepository.GetAll().ToList();

            vm.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }
    }
}