using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementWebAPI.Dto.Recipe;
using RecipeManagementWebAPI.Services;
using System.Security.Claims;

namespace RecipeManagementWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly ILogger<RecipeController> _logger;
        public RecipeController(
            IRecipeService recipeService,
            ILogger<RecipeController> logger
            )
        {
            _recipeService = recipeService;
            _logger = logger;
        }
        [HttpPost("create-recipe")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDto createRecipeDto)
        {
            try
            {
                // Retrieve the user ID from the token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                {
                    // Use the recipe service to create the recipe and associate it with the user
                    await _recipeService.CreateRecipeAsync(userId, createRecipeDto);

                    return Ok("Recipe created successfully");
                }

                return Unauthorized("Invalid or missing user ID in the token.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating recipe: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPut]
        [Authorize(Roles = "Admin")] // You can add specific roles or policies as needed
        public async Task<IActionResult> UpdateRecipe([FromBody] UpdateRecipeDto updateRecipeDto)
        {
            try
            {
                // Get the user id from the claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                {
                    // User not authenticated or invalid user id claim
                    return Unauthorized("Authentication failed or invalid user.");
                }

                // Call the service method to update the recipe
                var isUpdated = await _recipeService.UpdateRecipeAsync(userId, updateRecipeDto);

                if (isUpdated)
                {
                    return Ok("Recipe updated successfully.");
                }
                else
                {
                    return Forbid("You don't have the authority to update this recipe.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating recipe: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("allRecipes")]
      
        public async Task<IActionResult> GetAllRecipes()
        {
            try
            {
                // Call the service to get all recipes
                var recipes = await _recipeService.GetAllRecipesAsync();

                // Check if any recipes were found
                if (recipes != null && recipes.Any())
                {
                    return Ok(recipes); // Return the list of recipes
                }
                else
                {
                    return NotFound("No recipes found."); // Return a 404 status if no recipes were found
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all recipes: {ex.Message}");
                return StatusCode(500, "Internal Server Error"); // Handle the exception based on your application's requirements
            }
        }
        [HttpGet("categories")]
       
        public async Task<IActionResult> GetRecipesByCategories([FromQuery] IEnumerable<string> categories)
        {
            try
            {
                var recipes = await _recipeService.GetRecipesByCategoriesAsync(categories);

                if (recipes == null || !recipes.Any())
                {
                    return NotFound("No recipes found for the specified categories.");
                }

                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting recipes by categories: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetRecipesByCreatorName")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRecipesByCreatorName([FromQuery] string creatorName)
        {
            try
            {
                var recipes = await _recipeService.GetRecipesByCreatorNameAsync(creatorName);

                if (recipes == null || !recipes.Any())
                {
                    return NotFound("No recipes found for the specified creator name.");
                }

                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting recipes by creator name: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("title")]
        public async Task<IActionResult> GetRecipesByTitle([FromQuery] string title)
        {
            try
            {
                var recipes = await _recipeService.GetRecipesByTitleAsync(title);

                if (recipes == null || !recipes.Any())
                {
                    return NotFound("No recipes found for the specified title.");
                }

                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting recipes by title: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRecipesByPartial([FromQuery] string partialTitle)
        {
            try
            {
                var recipes = await _recipeService.SearchRecipesByPartialAsync(partialTitle);

                if (recipes == null || !recipes.Any())
                {
                    return NotFound("No recipes found for the specified partial title.");
                }

                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching recipes by partial title: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpDelete("{recipeId}")]
        [Authorize(Roles = "Admin")]// Assuming authorization is required for this action
        public async Task<IActionResult> DeleteRecipe(int recipeId)
        {
            try
            {
                // Get the user id from the claims
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var isDeleted = await _recipeService.DeleteRecipeAsync(userId, recipeId);

                if (!isDeleted)
                {
                    return NotFound($"Recipe with ID {recipeId} not found or you don't have the authority to delete it.");
                }

                return Ok($"Recipe with ID {recipeId} has been successfully deleted."); // Recipe deleted successfully
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting recipe: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
