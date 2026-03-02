using System.Diagnostics.CodeAnalysis;
using BlogApp.Models;
using BlogApp.Repository;
using BlogApp.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Service
{
    public class PostService : IPostService
    {
        private readonly IGenericRepository<Post> _postRepository;
        private readonly IWebHostEnvironment _env;

        private readonly string[] _allowedExt = { ".jpg", ".png", ".jpeg", ".webp" };

        public PostService(
            IGenericRepository<Post> postRepository,
            IWebHostEnvironment env)
        {
            _postRepository = postRepository;
            _env = env;
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
                         .Include(p => p.Likes);

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
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreatePostAsync(CreatePostViewModel vm, string userId)
        {
            var ext = Path.GetExtension(vm.FeatureImage.FileName).ToLower();

            if (!_allowedExt.Contains(ext))
                throw new Exception("Invalid Image Format");

            var post = vm.ToDataModel(vm);

            post.FeatureImagePath = await UploadFile(vm.FeatureImage);
            post.CreatedByUserId = userId;

            await _postRepository.AddAsync(post);
            await _postRepository.SaveAsync();
        }

        public async Task EditPostAsync(EditPostViewModel vm, string userId, bool isAdmin)
        {
            var existing = await _postRepository.GetByIdAsync(vm.post.Id);

            if (existing == null)
                throw new Exception("Post not found");

            if (existing.CreatedByUserId != userId && !isAdmin)
                throw new UnauthorizedAccessException();

            if (vm.FeatureImage != null)
            {
                var ext = Path.GetExtension(vm.FeatureImage.FileName).ToLower();
                if (!_allowedExt.Contains(ext))
                    throw new Exception("Invalid Image Format");

                vm.post.FeatureImagePath = await UploadFile(vm.FeatureImage);
            }
            else
            {
                vm.post.FeatureImagePath = existing.FeatureImagePath;
            }

            vm.post.CreatedByUserId = existing.CreatedByUserId;

            _postRepository.Update(vm.post);
            await _postRepository.SaveAsync();
        }

        
        public async Task DeletePostAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post == null)
                throw new Exception("Post not found");

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


        private async Task<string> UploadFile(IFormFile file)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string folderPath = Path.Combine(_env.WebRootPath, "images");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/images/" + fileName;
        }
    }
}