using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementWebAPI.Dto.Category;
using RecipeManagementWebAPI.Services;

namespace RecipeManagementWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(
            ICategoryService categoryService,
            ILogger<CategoryController> logger
            )
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            try
            {
                var categoryId = await _categoryService.CreateCategoryAsync(createCategoryDto.CategoryName);

                if (categoryId == -1)
                {
                    return BadRequest($"Category '{createCategoryDto.CategoryName}' already exists.");
                }

                return Ok("Category created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating category: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("allCategories")]

        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving categories: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                var isDeleted = await _categoryService.DeleteCategoryAsync(categoryId);

                if (!isDeleted)
                {
                    return NotFound($"Category with ID {categoryId} not found.");
                }

                return Ok("Category deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting category: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
