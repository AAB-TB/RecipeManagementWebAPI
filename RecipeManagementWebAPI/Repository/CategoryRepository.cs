using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using RecipeManagementWebAPI.Data;
using RecipeManagementWebAPI.Dto.Category;
using RecipeManagementWebAPI.Models;
using RecipeManagementWebAPI.Services;
using System.Data;

namespace RecipeManagementWebAPI.Repository
{
    public class CategoryRepository:ICategoryService
    {
        private readonly DapperContext _dapperContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryRepository> _logger;
       

        public CategoryRepository(
            DapperContext dapperContext,
            IMapper mapper,
            ILogger<CategoryRepository> logger
            )
        {
            _dapperContext = dapperContext;
            _mapper = mapper;
            _logger = logger;
           
        }

        public async Task<int> CreateCategoryAsync(string categoryName)
        {
            try
            {
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();
                    parameters.Add("@CategoryName", categoryName);

                    // Example SQL query for creating a category
                    string storedProcedure = "sp_CreateCategory";

                    // Execute the stored procedure
                    var affectedRows = await dbConnection.QueryFirstOrDefaultAsync<int>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating category: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }

        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();
                    parameters.Add("@CategoryId", categoryId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@Success", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    // Example SQL query for calling the stored procedure
                    string storedProcedure = "sp_DeleteCategory";

                    // Execute the stored procedure
                    await dbConnection.ExecuteAsync(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    // Retrieve the output parameter value
                    var success = parameters.Get<int>("@Success");

                    return success == 1; // Return true if the category was deleted successfully
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting category: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            try
            {
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();

                    // Example SQL query for fetching all categories
                    string storedProcedure = "sp_GetAllCategories";

                    // Execute the stored procedure
                    var categories = await dbConnection.QueryAsync<CategoryDto>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return categories;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving categories: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }
    }
}
