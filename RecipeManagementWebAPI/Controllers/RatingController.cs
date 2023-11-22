using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementWebAPI.Dto.Rating;
using RecipeManagementWebAPI.Dto.Recipe;
using RecipeManagementWebAPI.Services;
using System.Security.Claims;

namespace RecipeManagementWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly ILogger<RatingController> _logger;
        public RatingController(
            IRatingService ratingService,
            ILogger<RatingController> logger
            )
        {
            _ratingService = ratingService;
            _logger = logger;
        }

        [HttpPost("rate")]
        [Authorize(Roles = "Admin,Customer")] // To ensure only authenticated users can rate recipes
        public async Task<IActionResult> RateRecipe([FromBody] RatingDto rateRecipeDto)
        {
            try
            {
                // Get the user ID from the claims
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized("User not authenticated.");
                }

                // Call the RateRecipeAsync method from the service
                var isSuccess = await _ratingService.RateRecipeAsync(userId, rateRecipeDto.RecipeId, rateRecipeDto.RatingValue);

                if (isSuccess)
                {
                    return Ok("Recipe rated successfully.");
                }
                else
                {
                    return BadRequest("Unable to rate the recipe. Please check if the recipe exists, or you have already rated it, or you are trying to rate your own recipe.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error rating recipe: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
