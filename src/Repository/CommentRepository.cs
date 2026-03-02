using BlogApp.Data;
using BlogApp.Models;

namespace BlogApp.Repository
{
    public class CommentRepository : GenericRepository<Comment> ,ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<Comment> GetByPostId(int postId)
        {
            return  _context.Comments
                .Where(c => c.PostId == postId)
                .ToList();
        }
    }
}
