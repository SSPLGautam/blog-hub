namespace BlogApp.Core.Services
{
    public interface ILikeService
    {
        public Task<bool> ToggleLikeAsync(int postId, string userId);

        public Task<int> GetLikesCountByPostId(int postId);
    }
}
