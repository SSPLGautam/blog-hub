using BlogApp.Core.Data;
using BlogApp.Core.Data.Repositories;
using BlogApp.Core.Services;
using BlogApp.Models;

namespace BlogApp.Services;

public class CategoryService: ICategoryService
{
    private readonly ICategoryRepository _categoryRepo; 

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _categoryRepo = unitOfWork.CategoryRepository;
    }

    public List<Category>GetAllCategories() 
    {
        return  _categoryRepo.GetAll().ToList();
    }
}
