using BlogApp.Core.Domain;
using BlogApp.Models;

namespace BlogApp.Core.Data.Repositories;

public interface IPostRepository : IGenericRepository<Post>
{
    Task<(List<Post> posts, int totalCount)> GetPostsAsync(
    int? categoryId,
    bool isMostLiked,
    PostSortOrderEnum sortOrder,
    int page,
    int pageSize,
    string userId,
    bool isAdmin   , bool IncludeCategory,
    bool IncludeLikes , bool IncludeComment);
    IQueryable<Post> GetAllWithDetails();

    Task<Post> GetPostDetailAsync(int id);
}
