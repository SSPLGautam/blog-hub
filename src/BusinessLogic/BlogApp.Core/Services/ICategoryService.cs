using BlogApp.Models;

namespace BlogApp.Core.Services;

public interface ICategoryService
{
    public List<Category> GetAllCategories();
}
