using BlogApp.Models;
using BlogApp.Repository;

namespace BlogApp.Service
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
