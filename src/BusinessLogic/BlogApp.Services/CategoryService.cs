using BlogApp.Models;
using BlogApp.Core.Repositories;
using BlogApp.Core.Services;

namespace BlogApp.Services
{
    public class CategoryService: ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepo;

        public CategoryService(IGenericRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public List<Category>GetAllCategories() 
        {
            return  _categoryRepo.GetAll().ToList();
        }
    }
}
