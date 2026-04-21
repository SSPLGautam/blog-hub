using AutoMapper;
using BlogApp.Core.Services;
using BlogApp.Shared.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _categoryService.GetAllCategories();

            var result = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            });

            return Ok(result);
        }
    }
}