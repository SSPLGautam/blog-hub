using BlogApp.Models;

namespace BlogApp.Repository
{
    public interface ILikeRepository:IGenericRepository<PostLike>
    {
        PostLike GetUserLike(int postId, string userId);
        int CountByPostId(int postId);
    }
}
