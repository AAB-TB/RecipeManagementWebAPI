using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using RecipeManagementWebAPI.Data;
using RecipeManagementWebAPI.Dto.UserRole;
using RecipeManagementWebAPI.Services;

namespace RecipeManagementWebAPI.Repository
{
    public class UserRoleRepository:IUserRoleService
    {
        private readonly DapperContext _dapperContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryRepository> _logger;
    

        public UserRoleRepository(
            DapperContext dapperContext,
            IMapper mapper,
            ILogger<CategoryRepository> logger
          )
        {
            _dapperContext = dapperContext;
            _mapper = mapper;
            _logger = logger;
            
        }
      
        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
        {
            try
            {
                using (var connection = _dapperContext.GetDbConnection())
                {
                    connection.Open();

                    // Check if the role already exists for the user
                    var roleExistsQuery = "SELECT COUNT(*) FROM UserRoles WHERE UserId = @UserId AND RoleId = @RoleId;";
                    var roleExistsParameters = new { UserId = userId, RoleId = roleId };
                    var roleExists = await connection.ExecuteScalarAsync<int>(roleExistsQuery, roleExistsParameters);

                    if (roleExists > 0)
                    {
                        // The role already exists for the user
                        return false;
                    }

                    // Insert a new record in the UserRoles table
                    var insertUserRoleQuery = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);";
                    var insertUserRoleParameters = new { UserId = userId, RoleId = roleId };

                    // Execute the SQL query to insert the role for the user
                    await connection.ExecuteAsync(insertUserRoleQuery, insertUserRoleParameters);

                    // Return true to indicate successful role assignment
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions
                _logger.LogError($"Unexpected error during role assignment: {ex.Message}");
                // Rethrow the exception or handle it as appropriate for your application
                throw;
            }
        }

        public async Task<IEnumerable<UserRoleDto>> GetAllUsersWithRolesAsync()
        {
            try
            {
                using (var connection = _dapperContext.GetDbConnection())
                {
                    connection.Open();

                    // SQL query to retrieve users with their roles
                    var sqlQuery = "SELECT u.UserId, u.UserName, r.RoleName " +
                                   "FROM Users u " +
                                   "LEFT JOIN UserRoles ur ON u.UserId = ur.UserId " +
                                   "LEFT JOIN Roles r ON ur.RoleId = r.RoleId;";

                    // Execute the SQL query to fetch users with roles
                    var usersWithRoles = await connection.QueryAsync<UserRoleDto>(sqlQuery);

                    return usersWithRoles;
                }
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions
                _logger.LogError($"Unexpected error while fetching users with roles: {ex.Message}");
                // Rethrow the exception or handle it as appropriate for your application
                throw;
            }
        }

        public async Task<bool> RemoveUserRolesAsync(int userId, IEnumerable<int> roleIds)
        {
            try
            {
                using (var connection = _dapperContext.GetDbConnection())
                {
                    connection.Open();

                    // Delete specified roles for the user
                    var deleteQuery = "DELETE FROM UserRoles WHERE UserId = @UserId AND RoleId IN @RoleIds;";
                    await connection.ExecuteAsync(deleteQuery, new { UserId = userId, RoleIds = roleIds });

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions
                _logger.LogError($"Unexpected error during user role removal: {ex.Message}");
                // Rethrow the exception or handle it as appropriate for your application
                throw;
            }
        }

        public async Task<bool> UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds)
        {
            try
            {
                using (var connection = _dapperContext.GetDbConnection())
                {
                    connection.Open();

                    // Delete existing roles for the user
                    var deleteQuery = "DELETE FROM UserRoles WHERE UserId = @UserId;";
                    await connection.ExecuteAsync(deleteQuery, new { UserId = userId });

                    // Insert new roles for the user
                    var insertQuery = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);";
                    foreach (var roleId in roleIds)
                    {
                        await connection.ExecuteAsync(insertQuery, new { UserId = userId, RoleId = roleId });
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions
                _logger.LogError($"Unexpected error during user role update: {ex.Message}");
                // Rethrow the exception or handle it as appropriate for your application
                throw;
            }
        }

        Task<UserWithRolesDto> IUserRoleService.UserRolesCheckAsync(string userName)
        {
            throw new NotImplementedException();
        }
    }
}
