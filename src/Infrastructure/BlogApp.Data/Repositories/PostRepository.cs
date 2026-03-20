using BlogApp.Core.Data.Repositories;
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
}