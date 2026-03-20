using BlogApp.Models;

namespace BlogApp.Core.Data.Repositories;

public interface IPostRepository : IGenericRepository<Post>
{
    IQueryable<Post> GetAllWithDetails();

    Task<Post> GetPostDetailAsync(int id);
}
