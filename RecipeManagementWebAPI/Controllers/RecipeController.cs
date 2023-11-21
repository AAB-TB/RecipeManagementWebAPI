using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementWebAPI.Services;

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
    }
}
