using System.Security.Claims;
using BlogApp.Core.Repositories;
using BlogApp.Core.Services;
using BlogApp.Data.Repositories;
using BlogApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BlogApp.Services
{
    public class PostService : IPostService
    {
        private readonly IGenericRepository<Post> _postRepository;
        private readonly IHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<PostLike> _likeRepository;
        private readonly IGenericRepository<Comment> _commentRepository;
        private readonly string[] _allowedExt = { ".jpg", ".png", ".jpeg", ".webp" };

        public PostService(
            IGenericRepository<Post> postRepository,
            IGenericRepository<PostLike> likeRepository,
            IGenericRepository<Comment> commentRepository,
            IHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _postRepository = postRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(List<Post> posts, int totalCount)> GetPostsAsync(
            int? categoryId,
            bool isMostLiked,
            string sortOrder,
            int page,
            int pageSize,
            string userId,
            bool isAdmin)
        {
            var query = _postRepository.GetAll();

            query = query.Include(p => p.Category)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User);

            if (!isAdmin)
            {
                query = query.Where(p =>
                    p.IsPublished || p.CreatedByUserId == userId);
            }

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (isMostLiked)
            {
                query = query.OrderByDescending(p => p.Likes.Count)
                             .ThenByDescending(p => p.PublishedDate);
            }
            else
            {
                query = sortOrder switch
                {
                    "oldest" => query.OrderBy(p => p.PublishedDate),
                    "title_asc" => query.OrderBy(p => p.Title),
                    "title_desc" => query.OrderByDescending(p => p.Title),
                    _ => query.OrderByDescending(p => p.PublishedDate)
                };
            }

            int total = await query.CountAsync();

            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, total);
        }

        public async Task<Post> GetPostDetailAsync(int id)
        {
            return await _postRepository.GetAll()
                .Include(p => p.Category)

                .Include(p => p.Likes)
                .Include(p => p.Comments)
                  .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreatePostAsync(Post post)
        {
            var userId = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return;

            post.CreatedByUserId = userId;

            await _postRepository.AddAsync(post);
            await _postRepository.SaveAsync();
        }

        public async Task EditPostAsync(Post post)
        {
            var existing = await _postRepository.GetByIdAsync(post.Id);
            if (existing == null)
                throw new Exception("Post not found");

            var userId = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return;

            bool? isAdmin = _httpContextAccessor?.HttpContext?.User?.IsInRole("Admin");
            if (isAdmin == null)
                return;

            if (existing.CreatedByUserId != userId && !isAdmin.Value)
                throw new UnauthorizedAccessException();

            if (post.FeatureImagePath == null)
                post.FeatureImagePath = existing.FeatureImagePath;

            post.CreatedByUserId = existing.CreatedByUserId;

            _postRepository.Update(post);
            await _postRepository.SaveAsync();
        }

        public async Task DeletePostAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post == null)
                throw new Exception("Post not found");


            var likes = _likeRepository.GetAll()
                .Where(l => l.PostId == id)
                .ToList();

            if (likes.Any())
            {
                foreach (var like in likes)
                    _likeRepository.Delete(like);
            }


            var comments = _commentRepository.GetAll()
                .Where(c => c.PostId == id)
                .ToList();

            if (comments.Any())
            {
                foreach (var comment in comments)
                    _commentRepository.Delete(comment);
            }
            _postRepository.Delete(post);

            await _postRepository.SaveAsync();
        }


        public async Task TogglePublishAsync(int id, string userId, bool isAdmin)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post == null)
                throw new Exception("Post not found");

            if (post.CreatedByUserId != userId && !isAdmin)
                throw new UnauthorizedAccessException();

            post.IsPublished = !post.IsPublished;

            _postRepository.Update(post);
            await _postRepository.SaveAsync();
        }

        private async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string folderPath = Path.Combine(_env.ContentRootPath, "images");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/images/" + fileName;
        }
      
    }
}