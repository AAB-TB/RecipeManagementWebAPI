using RecipeManagementWebAPI.Dto.Category;

namespace RecipeManagementWebAPI.Services
{
    public interface ICategoryService
    {
        Task<int> CreateCategoryAsync(string categoryName);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    }
}
