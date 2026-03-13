using BlogApp.Models;

namespace BlogApp.Core.Repositories
{
    public interface ICommentRepository:IGenericRepository<Comment>
    {

        List<Comment> GetByPostId(int postId);

      
    }
}
