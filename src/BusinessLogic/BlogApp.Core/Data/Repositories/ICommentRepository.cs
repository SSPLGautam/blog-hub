using BlogApp.Models;

namespace BlogApp.Core.Data.Repositories
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<Comment?> GetByIdAsync(int id, bool includePost, bool includeUser);

        Task<List<Comment>> GetByPostIdAsync(int postId);
    }
}
