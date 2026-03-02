using BlogApp.Models;

namespace BlogApp.Service
{
    public interface ICategoryService
    {
        public List<Category> GetAllCategories();
    }
}
