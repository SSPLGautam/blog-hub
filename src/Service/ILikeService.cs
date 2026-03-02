namespace BlogApp.Service
{
    public interface ILikeService
    {

     public   Task<(bool liked, int count)> ToggleLikeAsync(int postId, string userId);
    }
}
