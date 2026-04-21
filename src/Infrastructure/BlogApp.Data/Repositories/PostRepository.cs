using BlogApp.Core.Data.Repositories;
using BlogApp.Core.Domain;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Repositories;

public class PostRepository : GenericRepository<Post>, IPostRepository
{
    private readonly AppDbContext _context;

    public PostRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    #region Repository Methods
    public IQueryable<Post> GetAllWithDetails()
    {
        return _context.Posts
            .Include(p => p.Category)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User);
    }

    public async Task<Post> GetPostDetailAsync(int id)
    {
        return await _context.Posts
            .Include(p => p.Category)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task<(List<Post> posts, int totalCount)> GetPostsAsync(
    int? categoryId,
    bool isMostLiked,
    PostSortOrderEnum sortOrder,
    int page,
    int pageSize,
    string userId,
    bool isAdmin,
    bool includeCategory,
    bool includeLikes,bool includeComment)
    {
        var query = _context.Posts.AsQueryable();

        if (includeCategory)
            query = query.Include(c => c.Category);
        if (includeLikes)
            query = query.Include(c => c.Likes);
        if (includeComment)
            query = query.Include(c => c.Comments);

        if (!isAdmin)
        {
            query = query.Where(p =>
                p.IsPublished || p.CreatedByUserId == userId);
        }

      
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        if (isMostLiked)
        {
            query = query
                .OrderByDescending(p => p.Likes.Count)
                .ThenByDescending(p => p.PublishedDate);
        }
        else
        {
            query = sortOrder switch
            {
                PostSortOrderEnum.Oldest => query.OrderBy(p => p.PublishedDate),
                PostSortOrderEnum.TitleAsc => query.OrderBy(p => p.Title),
                PostSortOrderEnum.TitleDesc => query.OrderByDescending(p => p.Title),
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
    #endregion
} 