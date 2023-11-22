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
                // Ensure the rating value is between 1 and 5
                if (ratingValue < 1 || ratingValue > 5)
                {
                    // Handle invalid rating value
                    return false;
                }

                // Use Dapper connection for SQL queries
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Check if the user has already rated the recipe
                    var checkRatingSql = "SELECT COUNT(*) FROM Ratings WHERE UserId = @UserId AND RecipeId = @RecipeId";
                    var existingRatingCount = await dbConnection.ExecuteScalarAsync<int>(checkRatingSql, new { UserId = userId, RecipeId = recipeId });

                    if (existingRatingCount > 0)
                    {
                        // User has already rated this recipe
                        return false;
                    }

                    // Check if the user is trying to rate their own recipe
                    var checkRecipeCreatorSql = "SELECT UserId FROM Recipes WHERE RecipeId = @RecipeId";
                    var recipeCreatorId = await dbConnection.ExecuteScalarAsync<int>(checkRecipeCreatorSql, new { RecipeId = recipeId });

                    if (recipeCreatorId == userId)
                    {
                        // User cannot rate their own recipe
                        return false;
                    }

                    // Insert the new rating into the database
                    var insertRatingSql = "INSERT INTO Ratings (RecipeId, UserId, RatingValue) VALUES (@RecipeId, @UserId, @RatingValue)";
                    await dbConnection.ExecuteAsync(insertRatingSql, new { RecipeId = recipeId, UserId = userId, RatingValue = ratingValue });

                    // Update the average rating for the recipe
                    var updateAverageRatingSql = @"
                UPDATE Recipes 
                SET AverageRating = (
                    SELECT AVG(CAST(RatingValue AS DECIMAL(3, 2))) 
                    FROM Ratings 
                    WHERE RecipeId = @RecipeId
                )
                WHERE RecipeId = @RecipeId";
                    await dbConnection.ExecuteAsync(updateAverageRatingSql, new { RecipeId = recipeId });

                    return true;
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
