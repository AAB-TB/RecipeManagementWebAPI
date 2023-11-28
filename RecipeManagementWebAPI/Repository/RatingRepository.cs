using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using RecipeManagementWebAPI.Data;
using RecipeManagementWebAPI.Services;
using System.Data;

namespace RecipeManagementWebAPI.Repository
{
    public class RatingRepository:IRatingService
    {
        private readonly DapperContext _dapperContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryRepository> _logger;
       


        public RatingRepository(
            DapperContext dapperContext,
            IMapper mapper,
            ILogger<CategoryRepository> logger
            )
        {
            _dapperContext = dapperContext;
            _mapper = mapper;
            _logger = logger;
            
        }

        public async Task<bool> RateRecipeAsync(int userId, int recipeId, int ratingValue)
        {
            try
            {
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();
                    parameters.Add("@RecipeId", recipeId);
                    parameters.Add("@UserId", userId);
                    parameters.Add("@RatingValue", ratingValue);

                    // Example SQL query for rating the recipe using a stored procedure
                    string storedProcedure = "sp_RateRecipe";

                    // Execute the stored procedure
                    var ratingStatus = await dbConnection.QueryFirstOrDefaultAsync<int>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    // Check the result of the stored procedure and return accordingly
                    return ratingStatus switch
                    {
                        1 => true,   // Rating successful
                        0 => false,  // User cannot rate their own recipe
                        -1 => false, // User has already rated this recipe
                        -2 => false, // Invalid rating value
                        _ => false   // Recipe not found
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error rating recipe: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }
    }
}
