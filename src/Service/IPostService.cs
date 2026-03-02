using BlogApp.Models;
using BlogApp.ViewModel;

namespace BlogApp.Service
{
    public interface IPostService
    {
    
        Task<(List<Post> posts, int totalCount)> GetPostsAsync(
        int? categoryId,
        bool isMostLiked,
        string sortOrder,
        int page,
        int pageSize,
        string userId,
        bool isAdmin);

        Task<Post> GetPostDetailAsync(int id);

        Task CreatePostAsync(CreatePostViewModel vm, string userId);

        Task EditPostAsync(EditPostViewModel vm, string userId, bool isAdmin);

        Task DeletePostAsync(int id);

       

        Task TogglePublishAsync(int id, string userId, bool isAdmin);

        

        








    }
}
