using BlogApp.Models;

namespace BlogApp.Core.Repositories
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<Comment> GetCommentWithPostAsync(int id);
        Task<List<Comment>> GetByPostIdAsync(int postId);


    }
}
