using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using RecipeManagementWebAPI.Data;
using RecipeManagementWebAPI.Dto.Role;
using RecipeManagementWebAPI.Models;
using RecipeManagementWebAPI.Services;

namespace RecipeManagementWebAPI.Repository
{
    public class RoleRepository:IRoleService
    {
        private readonly DapperContext _dapperContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryRepository> _logger;
        

        public RoleRepository(
            DapperContext dapperContext,
            IMapper mapper,
            ILogger<CategoryRepository> logger
            )
        {
            _dapperContext = dapperContext;
            _mapper = mapper;
            _logger = logger;
           
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            try
            {
                using (var connection = _dapperContext.GetDbConnection())
                {
                    connection.Open();

                    // Check if the role already exists
                    var roleExistsQuery = "SELECT COUNT(*) FROM Roles WHERE RoleName = @RoleName;";
                    var roleExists = await connection.ExecuteScalarAsync<int>(roleExistsQuery, new { RoleName = roleName });

                    if (roleExists > 0)
                    {
                        // The role already exists, return false to indicate failure
                        return false;
                    }

                    // Role does not exist, insert the new role
                    var insertRoleQuery = "INSERT INTO Roles (RoleName) VALUES (@RoleName);";
                    var affectedRows = await connection.ExecuteAsync(insertRoleQuery, new { RoleName = roleName });

                    // Return true if at least one row was affected, indicating success
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions
                _logger.LogError($"Unexpected error while creating role: {ex.Message}");
                // Return false to indicate failure in case of an exception
                return false;
            }
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            try
            {
                using (var connection = _dapperContext.GetDbConnection())
                {
                    connection.Open();

                    // Check if the role exists
                    var roleExistsQuery = "SELECT COUNT(*) FROM Roles WHERE RoleId = @RoleId;";
                    var roleExists = await connection.ExecuteScalarAsync<int>(roleExistsQuery, new { RoleId = roleId });

                    if (roleExists == 0)
                    {
                        // The role does not exist, return false or handle as needed
                        return false; // For example, returning false to indicate that the role does not exist
                    }

                    // Role exists, delete the role
                    var deleteRoleQuery = "DELETE FROM Roles WHERE RoleId = @RoleId;";
                    var affectedRows = await connection.ExecuteAsync(deleteRoleQuery, new { RoleId = roleId });

                    // Return true if at least one row was affected (role deleted), false otherwise
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions
                _logger.LogError($"Unexpected error while deleting role: {ex.Message}");
                // Rethrow the exception or handle it as appropriate for your application
                throw;
            }
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {

            try
            {
                using (var connection = _dapperContext.GetDbConnection())
                {
                    connection.Open();

                    // SQL query to select all roles
                    var sqlQuery = "SELECT * FROM Roles;";

                    // Execute the SQL query and retrieve roles
                    var roles = await connection.QueryAsync<Role>(sqlQuery);

                    // Map Role to RoleDto using _mapper
                    var roleDtoList = _mapper.Map<IEnumerable<RoleDto>>(roles);

                    return roleDtoList;
                }
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions
                _logger.LogError($"Unexpected error while retrieving roles: {ex.Message}");
                // Rethrow the exception or handle it as appropriate for your application
                throw;
            }
        }
    }
}
