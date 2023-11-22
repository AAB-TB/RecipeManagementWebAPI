using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using RecipeManagementWebAPI.Data;
using RecipeManagementWebAPI.Dto.Recipe;
using RecipeManagementWebAPI.Services;
using System.Data;

namespace RecipeManagementWebAPI.Repository
{
    public class RecipeRepository:IRecipeService
    {
        private readonly DapperContext _dapperContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryRepository> _logger;
        

        public RecipeRepository(
            DapperContext dapperContext,
            IMapper mapper,
            ILogger<CategoryRepository> logger
            )
        {
            _dapperContext = dapperContext;
            _mapper = mapper;
            _logger = logger;
            
        }

        public async Task<bool> CreateRecipeAsync(int userId,CreateRecipeDto createRecipeDto)
        {
            try
            {
                // Use Dapper connection for SQL query
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Define the SQL query to insert the new recipe into the database and associate it with the user
                    string insertRecipeSql = @"
                INSERT INTO Recipes (Title, Description, Ingredients, CategoryId, UserId)
                VALUES (@Title, @Description, @Ingredients, @CategoryId, @UserId);
            ";

                    // Execute the SQL query to insert the new recipe into the database
                    int affectedRows = await dbConnection.ExecuteAsync(insertRecipeSql, new
                    {
                        createRecipeDto.Title,
                        createRecipeDto.Description,
                        createRecipeDto.Ingredients,
                        createRecipeDto.CategoryId,
                        UserId = userId
                    });

                    // Return true if at least one row was affected (recipe created successfully), otherwise return false
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating recipe: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }

        public async Task<bool> DeleteRecipeAsync(int userId, int recipeId)
        {
            try
            {
                // Use Dapper connection for SQL query
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Check if the recipe exists and belongs to the logged-in user
                    var checkRecipeExistsSql = "SELECT * FROM Recipes WHERE RecipeId = @RecipeId AND UserId = @UserId";
                    var existingRecipe = await dbConnection.QueryFirstOrDefaultAsync<RecipeDto>(checkRecipeExistsSql,
                        new { RecipeId = recipeId, UserId = userId });

                    if (existingRecipe == null)
                    {
                        // Recipe not found or does not belong to the user
                        return false;
                    }

                    // SQL query for deleting the recipe
                    var deleteRecipeSql = "DELETE FROM Recipes WHERE RecipeId = @RecipeId AND UserId = @UserId";

                    // Delete the recipe
                    var affectedRows = await dbConnection.ExecuteAsync(deleteRecipeSql, new { RecipeId = recipeId, UserId = userId });

                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting recipe: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }

        public async Task<IEnumerable<RecipeDto>> GetAllRecipesAsync()
        {

            try
            {
                // Use Dapper connection for SQL query
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // SQL query to get all recipes with category name and user name
                    var getAllRecipesSql = @"
                SELECT 
                    r.RecipeId, 
                    r.Title, 
                    r.Description, 
                    r.Ingredients, 
                    r.CategoryId, 
                    r.UserId, 
                    r.AverageRating,
                    c.CategoryName, 
                    u.Username as UserName
                FROM Recipes r
                INNER JOIN Categories c ON r.CategoryId = c.CategoryId
                INNER JOIN Users u ON r.UserId = u.UserId";

                    // Execute the query and retrieve the recipes
                    var recipes = await dbConnection.QueryAsync<RecipeDto>(getAllRecipesSql);

                    return recipes;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all recipes: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }

        public async Task<IEnumerable<RecipeDto>> GetRecipesByCategoriesAsync(IEnumerable<string> categories)
        {
            try
            {
                // Use Dapper connection for SQL query
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // SQL query to get recipes by category names
                    var getRecipesByCategoriesSql = @"
                SELECT 
                    r.RecipeId, 
                    r.Title, 
                    r.Description, 
                    r.Ingredients, 
                    r.CategoryId, 
                    r.UserId, 
                    r.AverageRating,
                    c.CategoryName, 
                    u.Username as UserName
                FROM Recipes r
                INNER JOIN Categories c ON r.CategoryId = c.CategoryId
                INNER JOIN Users u ON r.UserId = u.UserId
                WHERE c.CategoryName IN @Categories";

                    // Execute the query and retrieve the recipes
                    var recipes = await dbConnection.QueryAsync<RecipeDto>(getRecipesByCategoriesSql, new { Categories = categories });

                    return recipes;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting recipes by categories: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }

        public async Task<IEnumerable<RecipeDto>> GetRecipesByCreatorNameAsync(string creatorName)
        {
            try
            {
                // Use Dapper connection for SQL query
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // SQL query to get recipes by creator name
                    var getRecipesByCreatorNameSql = "SELECT * FROM Recipes WHERE UserId IN (SELECT UserId FROM Users WHERE LOWER(Username) = LOWER(@CreatorName))";

                    // Execute the query and retrieve the recipes
                    var recipes = await dbConnection.QueryAsync<RecipeDto>(getRecipesByCreatorNameSql, new { CreatorName = creatorName });

                    return recipes;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting recipes by creator name: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }

        public async Task<IEnumerable<RecipeDto>> GetRecipesByTitleAsync(string title)
        {
            try
            {
                // Use Dapper connection for SQL query
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // SQL query to get recipes by title
                    var getRecipesByTitleSql = "SELECT * FROM Recipes WHERE LOWER(Title) = LOWER(@Title)";

                    // Execute the query and retrieve the recipes
                    var recipes = await dbConnection.QueryAsync<RecipeDto>(getRecipesByTitleSql, new { Title = title });

                    return recipes;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting recipes by title: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }

        public async Task<IEnumerable<RecipeDto>> SearchRecipesByPartialAsync(string partialTitle)
        {
            try
            {
                // Use Dapper connection for SQL query
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // SQL query to search recipes by partial title
                    var searchRecipesByPartialSql = "SELECT * FROM Recipes WHERE LOWER(Title) LIKE LOWER(@PartialTitle)";

                    // Execute the query and retrieve the recipes
                    var recipes = await dbConnection.QueryAsync<RecipeDto>(searchRecipesByPartialSql, new { PartialTitle = $"%{partialTitle}%" });

                    return recipes;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching recipes by partial title: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }

        public async Task<bool> UpdateRecipeAsync(int userId, UpdateRecipeDto updateRecipeDto)
        {
            try
            {
                // Use Dapper connection for SQL query
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Check if the recipe exists and belongs to the logged-in user
                    var checkRecipeExistsSql = "SELECT * FROM Recipes WHERE RecipeId = @RecipeId AND UserId = @UserId";
                    var existingRecipe = await dbConnection.QueryFirstOrDefaultAsync<RecipeDto>(checkRecipeExistsSql,
                        new { updateRecipeDto.recipeId, UserId = userId });

                    if (existingRecipe == null)
                    {
                        // Recipe not found or does not belong to the user
                        return false;
                    }

                    // Ensure that the user updating the recipe is the same user who created it
                    if (existingRecipe.UserId != userId)
                    {
                        return false;
                    }

                    // SQL query for updating the recipe
                    var updateRecipeSql = @"
                UPDATE Recipes 
                SET Title = @Title, 
                    Description = @Description, 
                    Ingredients = @Ingredients, 
                    CategoryId = @CategoryId 
                WHERE RecipeId = @RecipeId AND UserId = @UserId";

                    // Update the recipe
                    var affectedRows = await dbConnection.ExecuteAsync(updateRecipeSql, new
                    {
                        updateRecipeDto.recipeId,
                        updateRecipeDto.Title,
                        updateRecipeDto.Description,
                        updateRecipeDto.Ingredients,
                        updateRecipeDto.CategoryId,
                        UserId = userId
                    });

                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating recipe: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }
    }
}
