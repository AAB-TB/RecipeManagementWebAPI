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
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();
                    parameters.Add("@Title", createRecipeDto.Title);
                    parameters.Add("@Description", createRecipeDto.Description);
                    parameters.Add("@Ingredients", createRecipeDto.Ingredients);
                    parameters.Add("@CategoryId", createRecipeDto.CategoryId);
                    parameters.Add("@UserId", userId);

                    // Example SQL query for creating a recipe
                    string storedProcedure = "sp_CreateRecipe";

                    // Execute the stored procedure
                    int affectedRows = await dbConnection.ExecuteAsync(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

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
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();
                    parameters.Add("@RecipeId", recipeId);
                    parameters.Add("@UserId", userId);
                    parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                    // Example SQL query for calling the stored procedure
                    string storedProcedure = "sp_DeleteRecipe";

                    // Execute the stored procedure
                    await dbConnection.ExecuteAsync(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    // Retrieve the value of the output parameter
                    bool success = parameters.Get<bool>("@Success");

                    return success; // Return true if the recipe was deleted

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
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();

                    // Example SQL query for fetching all recipes
                    string storedProcedure = "sp_GetAllRecipes";

                    // Execute the stored procedure
                    var recipes = await dbConnection.QueryAsync<RecipeDto>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

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
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();
                    parameters.Add("@CategoriesList", string.Join(",", categories));

                    // Example SQL query for fetching recipes by categories
                    string storedProcedure = "sp_GetRecipesByCategories";

                    // Execute the stored procedure
                    var recipes = await dbConnection.QueryAsync<RecipeDto>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

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
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();
                    parameters.Add("@CreatorName", creatorName);

                    // Example SQL query for fetching recipes by creator name
                    string storedProcedure = "sp_GetRecipesByCreatorName";

                    // Execute the stored procedure
                    var recipes = await dbConnection.QueryAsync<RecipeDto>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

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
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();
                    parameters.Add("@Title", title);

                    // Example SQL query for fetching recipes by title
                    string storedProcedure = "sp_GetRecipesByTitle";

                    // Execute the stored procedure
                    var recipes = await dbConnection.QueryAsync<RecipeDto>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

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
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();
                    parameters.Add("@PartialTitle", partialTitle);

                    // Example SQL query for searching recipes by partial title
                    string storedProcedure = "sp_SearchRecipesByPartial";

                    // Execute the stored procedure
                    var recipes = await dbConnection.QueryAsync<RecipeDto>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

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
        using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
        {
            dbConnection.Open();

            // Use dynamic parameters
            var parameters = new DynamicParameters();
            parameters.Add("@RecipeId", updateRecipeDto.RecipeId);
            parameters.Add("@UserId", userId);
            parameters.Add("@Title", updateRecipeDto.Title);
            parameters.Add("@Description", updateRecipeDto.Description);
            parameters.Add("@Ingredients", updateRecipeDto.Ingredients);
            parameters.Add("@CategoryId", updateRecipeDto.CategoryId);

            // Example SQL query for updating the recipe using a stored procedure
            string storedProcedure = "sp_UpdateRecipe";

            // Execute the stored procedure
            var updateStatus = await dbConnection.QueryFirstOrDefaultAsync<int>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Check the result of the stored procedure and return accordingly
            return updateStatus switch
            {
                1 => true,   // Recipe updated successfully
                0 => false,  // Recipe does not belong to the user
                _ => false   // Recipe not found
            };
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
