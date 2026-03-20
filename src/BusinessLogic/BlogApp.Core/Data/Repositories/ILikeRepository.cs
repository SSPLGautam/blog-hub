using BlogApp.Models;

namespace BlogApp.Core.Data.Repositories;

public interface ILikeRepository : IGenericRepository<PostLike>
{
    Task<PostLike?> GetLikeByUserId(int postId, string userId);
    int GetLikesCountByPostId(int postId);
}
