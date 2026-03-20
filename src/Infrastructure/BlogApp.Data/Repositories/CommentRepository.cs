using BlogApp.Core.Data.Repositories;
using BlogApp.Data;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Repositories;

public class CommentRepository : GenericRepository<Comment> ,ICommentRepository
{
    private readonly AppDbContext _context;

    public CommentRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }
    
    public async Task<List<Comment>> GeetByPostIdAsync(int postId)
    {
        return await _context.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.User)
            .ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(int id, bool includePost, bool includeUser)
    {
        var query = _context.Comments.AsQueryable();

        query = query.Where(c => c.Id == id);

        if (includePost)
            query = query.Include(c => c.Post);
        if (includeUser)
            query = query.Include(c => c.User);

        return await query.FirstOrDefaultAsync();

    }
}
