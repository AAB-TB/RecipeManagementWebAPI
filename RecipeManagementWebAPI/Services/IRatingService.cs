
namespace RecipeManagementWebAPI.Services
{
    public interface IRatingService
    {
        Task<bool> RateRecipeAsync(int userId, int recipeId, int rating);
    }
}
