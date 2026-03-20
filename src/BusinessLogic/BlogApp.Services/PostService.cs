using System.Security.Claims;
using BlogApp.Core.Data;
using BlogApp.Core.Data.Repositories;
using BlogApp.Core.Services;
using BlogApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BlogApp.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPostRepository _postsRepository;
    private readonly IHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly string[] _allowedExt = { ".jpg", ".png", ".jpeg", ".webp" };

    public PostService(
        IUnitOfWork unitOfWork,
        IHostEnvironment env,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _postsRepository = unitOfWork.PostsRepository;
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
        var query = _postsRepository.GetAllWithDetails();

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
        return await _postsRepository.GetPostDetailAsync(id);
    }

    public async Task CreatePostAsync(Post post)
    {
        var userId = _httpContextAccessor?.HttpContext?.User?
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return;

        post.CreatedByUserId = userId;

        await _postsRepository.AddAsync(post);
      
    }

    public async Task EditPostAsync(Post post)
    {
        var existing = await _postsRepository.GetByIdAsync(post.Id);

        if (existing == null)
            throw new Exception("Post not found");

        var userId = _httpContextAccessor?.HttpContext?.User?
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return;

        bool isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

        if (existing.CreatedByUserId != userId && !isAdmin)
            throw new UnauthorizedAccessException();

        if (post.FeatureImagePath == null)
            post.FeatureImagePath = existing.FeatureImagePath;

        post.CreatedByUserId = existing.CreatedByUserId;

        _postsRepository.Update(post);
       
    }

    public async Task DeletePostAsync(int id)
    {
        var post = await _postsRepository.GetByIdAsync(id);

        if (post == null)
            throw new Exception("Post not found");

        var likes = _unitOfWork.LikesRepository.GetAll()
            .Where(l => l.PostId == id)
            .ToList();

        foreach (var like in likes)
            _unitOfWork.LikesRepository.Delete(like);

        var comments = _unitOfWork.CommentsRepository.GetAll()
            .Where(c => c.PostId == id)
            .ToList();

        foreach (var comment in comments)
            _unitOfWork.CommentsRepository.Delete(comment);

        _postsRepository.Delete(post);

    }

    public async Task TogglePublishAsync(int id, string userId, bool isAdmin)
    {
        var post = await _postsRepository.GetByIdAsync(id);

        if (post == null)
            throw new Exception("Post not found");

        if (post.CreatedByUserId != userId && !isAdmin)
            throw new UnauthorizedAccessException();

        post.IsPublished = !post.IsPublished;

        _postsRepository.Update(post);
    
    }

    private async Task<string> UploadFile(IFormFile file)
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