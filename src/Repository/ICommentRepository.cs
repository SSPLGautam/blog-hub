using BlogApp.Models;

namespace BlogApp.Repository
{
    public interface ICommentRepository:IGenericRepository<Comment>
    {

        List<Comment> GetByPostId(int postId);
    }
}
