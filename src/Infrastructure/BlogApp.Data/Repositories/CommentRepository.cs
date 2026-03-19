using BlogApp.Core.Repositories;
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
    public async Task<Comment> GetCommentWithPostAsync(int id)
    {
        return await _context.Comments
            .Include(c => c.Post)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task<List<Comment>> GetByPostIdAsync(int postId)
    {
        return await _context.Comments
              .Include(c => c.User)
              .Include(c => c.Post)
              .Where(c => c.PostId == postId)
              .ToListAsync();
    }
}
