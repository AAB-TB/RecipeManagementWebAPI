using RecipeManagementWebAPI.Dto.Recipe;

namespace RecipeManagementWebAPI.Services
{
    public interface IRecipeService
    {
        Task<bool> CreateRecipeAsync(int userId,CreateRecipeDto createRecipeDto);
        Task<bool> DeleteRecipeAsync(int userId, int recipeId);
        Task<bool> UpdateRecipeAsync(int userId, UpdateRecipeDto updateRecipeDto);
        Task<IEnumerable<RecipeDto>> GetAllRecipesAsync();
        Task<IEnumerable<RecipeDto>> GetRecipesByCreatorNameAsync(string creatorName);
        Task<IEnumerable<RecipeDto>> GetRecipesByTitleAsync(string title);
        Task<IEnumerable<RecipeDto>> GetRecipesByCategoriesAsync(IEnumerable<string> categories);
        Task<IEnumerable<RecipeDto>> SearchRecipesByPartialAsync(string partialTitle);
    }
}
