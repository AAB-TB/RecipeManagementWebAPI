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

                    // Check if the category already exists
                    var checkCategoryQuery = "SELECT * FROM Categories WHERE CategoryName = @CategoryName";
                    var existingCategory = await dbConnection.QueryFirstOrDefaultAsync<Category>(
                        checkCategoryQuery,
                        new { CategoryName = categoryName }
                    );

                    if (existingCategory != null)
                    {
                        _logger.LogWarning($"Category '{categoryName}' already exists.");
                        return -1; // Category already exists
                    }

                    // Insert the new category into the database
                    var insertCategoryQuery = "INSERT INTO Categories (CategoryName) VALUES (@CategoryName)";
                    var affectedRows = await dbConnection.ExecuteAsync(
                        insertCategoryQuery,
                        new { CategoryName = categoryName }
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

                    // Check if the category exists before attempting to delete
                    var checkCategoryQuery = "SELECT * FROM Categories WHERE CategoryId = @CategoryId";
                    var existingCategory = await dbConnection.QueryFirstOrDefaultAsync<Category>(
                        checkCategoryQuery,
                        new { CategoryId = categoryId }
                    );

                    if (existingCategory == null)
                    {
                        _logger.LogWarning($"Category with ID {categoryId} does not exist.");
                        return false; // Category does not exist
                    }

                    // Delete the category from the database
                    var deleteCategoryQuery = "DELETE FROM Categories WHERE CategoryId = @CategoryId";
                    var affectedRows = await dbConnection.ExecuteAsync(
                        deleteCategoryQuery,
                        new { CategoryId = categoryId }
                    );

                    return affectedRows > 0; // Return true if at least one row was affected (category deleted)
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

                    // Retrieve all categories from the database
                    var getAllCategoriesQuery = "SELECT * FROM Categories";
                    var categories = await dbConnection.QueryAsync<CategoryDto>(getAllCategoriesQuery);

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
