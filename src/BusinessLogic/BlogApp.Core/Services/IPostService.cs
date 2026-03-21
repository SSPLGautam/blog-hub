using BlogApp.Core.Domain;
using BlogApp.Models;
using Microsoft.AspNetCore.Http;

namespace BlogApp.Core.Services
{
    public interface IPostService
    {
        #region Post Methood
        Task<(List<Post> posts, int totalCount)> GetPostsAsync(
     int? categoryId,
     bool isMostLiked,
     PostSortOrderEnum sortOrder,
     int page,
     int pageSize,
     string userId,
     bool isAdmin);

        Task<Post> GetPostDetailAsync(int id);

        Task CreatePostAsync(Post post);

        Task EditPostAsync(Post post);
        Task DeletePostAsync(int id);

        #endregion

        Task TogglePublishAsync(int id, string userId, bool isAdmin);

        

        








    }
}
