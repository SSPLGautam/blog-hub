using System.Diagnostics.Eventing.Reader;
using BlogApp.Data;
using BlogApp.Models;
using BlogApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{

    [Route("[Controller]")]

    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly string[] _allowExtension = { ".jpg", ".png", ".jpeg", ".webp" };
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("")]
        public IActionResult Index(int? categoryId, int page = 1)
        {
            int pageSize = 6; // posts per page

            var postQuery = _context.Posts
                                    .Include(p => p.Category)
                                    .AsQueryable();

            if (categoryId.HasValue)
            {
                postQuery = postQuery.Where(p => p.CategoryId == categoryId);
            }

            int totalPosts = postQuery.Count();

            var posts = postQuery
                        .OrderByDescending(p => p.PublishedDate)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
            ViewBag.CategoryId = categoryId;

            ViewBag.Categories = _context.Categories.ToList();

            return View(posts);
        }


        [HttpGet("Detail")]

        public async Task<IActionResult> Detail(int id)
        {
            if (id == null)
            {
                return NotFound();

            }
            var post = _context.Posts.Include(p => p.Category).Include(p => p.Comments)
              .FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }


        [HttpGet("Create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var postViewModel = new PostViewModel();
            postViewModel.Categories = _context.Categories.Select(c =>
            new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,

            }
            ).ToList();
            return View(postViewModel);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var inputFileExtension = Path.GetExtension(postViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = _allowExtension.Contains(inputFileExtension);


                if (!isAllowed)
                {
                    ModelState.AddModelError("", "Invalid Image Format. Allowed: .jpg, .jpeg, .png, .webp");
                    return View(postViewModel);
                }

                postViewModel.post.FeatureImagePath =
                    await UploadFileFolder(postViewModel.FeatureImage);

                await _context.Posts.AddAsync(postViewModel.post);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            postViewModel = new PostViewModel();
            postViewModel.Categories = _context.Categories.Select(c =>
            new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,


            }
            ).ToList();

            return View(postViewModel);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(int id)
        {

            if (id == null) return NotFound();

            var postFromDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (postFromDb == null) return NotFound();
            EditPostViewModel editViewModel = new EditPostViewModel
            {

                post = postFromDb,
                Categories = _context.Categories.Select(c =>
                   new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                   {
                       Value = c.Id.ToString(),
                       Text = c.Name,
                   }



              ).ToList()
            };
            return View(editViewModel);
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(EditPostViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }
            var postFromDb = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == editViewModel.post.Id);
            if (postFromDb == null) return NotFound();

            if (editViewModel.FeatureImage != null)
            {

                var inputFileExtension = Path.GetExtension(editViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = _allowExtension.Contains(inputFileExtension);


                if (!isAllowed)
                {
                    ModelState.AddModelError("", "Invalid Image Format. Allowed: .jpg, .jpeg, .png, .webp");
                    return View(editViewModel);
                }
                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", Path.GetFileName(postFromDb.FeatureImagePath));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
                editViewModel.post.FeatureImagePath = await UploadFileFolder(editViewModel.FeatureImage);

            }
            else
            {
                editViewModel.post.FeatureImagePath = postFromDb.FeatureImagePath;
            }
            _context.Posts.Update(editViewModel.post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(int id)
        {

            var postFromDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (postFromDb == null) return NotFound();
            return View(postFromDb);

        }

        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteConfrim(int id)
        {
            if (id == null) return NotFound();
            var postFromDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if (string.IsNullOrEmpty(postFromDb.FeatureImagePath))
            {

                var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", Path.GetFileName(postFromDb.FeatureImagePath));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }

            }
            _context.Posts.Remove(postFromDb);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Post deleted successfully!";
            return RedirectToAction("Index");


        }

        [HttpPost("AddComment")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] Comment comment)
        {
            if (comment == null)
            {
                return BadRequest();
            }
            comment.UserName = User.Identity.Name;  //add ab Now
            comment.CommentDate = DateTime.Now;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Json(new
            {
                username = comment.UserName,
                content = comment.Content,
                commentDate = comment.CommentDate.ToString("dd MMM yyyy")
            });
        }
        [HttpPost("DeleteComment")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok();
        }


        private async Task<string> UploadFileFolder(IFormFile file)
        {
            var inputFileExtension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + inputFileExtension;
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var imagesFolderPath = Path.Combine(wwwRootPath, "images");

            if (!Directory.Exists(imagesFolderPath))
            {

                Directory.CreateDirectory(imagesFolderPath);

            }

            var filePath = Path.Combine(imagesFolderPath, fileName);
            try
            {
                await using (var fileStrean = new FileStream(filePath, FileMode.Create))
                {

                    await file.CopyToAsync(fileStrean);
                }
            }
            catch (Exception ex)
            {

                return "Error uploading Image " + ex.Message;


            }
            return "/images/" + fileName;
        }


    }
}

