using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using RecipeManagementWebAPI.Data;
using RecipeManagementWebAPI.Dto.UserRole;
using RecipeManagementWebAPI.Services;
using System.Data;

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

                    // Create DynamicParameters for role existence check
                    var roleExistsParameters = new { UserId = userId, RoleId = roleId };

                    // Execute the stored procedure to check if the role already exists
                    var roleExists = await connection.QueryFirstOrDefaultAsync<int>("sp_AssignRoleToUser", roleExistsParameters, commandType: CommandType.StoredProcedure);

                    if (roleExists == 0)
                    {
                        // The role already exists for the user
                        return false;
                    }

                    // Create DynamicParameters for inserting the role
                    var insertUserRoleParameters = new { UserId = userId, RoleId = roleId };

                    // Execute the stored procedure to insert the role for the user
                    await connection.ExecuteAsync("sp_AssignRoleToUser", insertUserRoleParameters, commandType: CommandType.StoredProcedure);

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

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();

                    // Example SQL query for fetching all users with roles
                    string storedProcedure = "sp_GetAllUsersWithRoles";

                    // Execute the stored procedure
                    var usersWithRoles = await connection.QueryAsync<UserRoleDto>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure);

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

                    // Convert the list of role IDs to a comma-separated string
                    var roleIdsString = string.Join(",", roleIds);

                    // Create DynamicParameters for the stored procedure
                    var parameters = new DynamicParameters();
                    parameters.Add("UserId", userId);
                    parameters.Add("RoleIdsString", roleIdsString);

                    // Execute the stored procedure to remove roles for the user
                    await connection.ExecuteAsync("sp_RemoveUserRoles", parameters, commandType: CommandType.StoredProcedure);

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

                    // Convert the list of role IDs to a comma-separated string
                    var roleIdsString = string.Join(",", roleIds);

                    // Create DynamicParameters for the stored procedure
                    var parameters = new DynamicParameters();
                    parameters.Add("UserId", userId);
                    parameters.Add("RoleIdsString", roleIdsString);

                    // Execute the stored procedure to update roles for the user
                    await connection.ExecuteAsync("sp_UpdateUserRoles", parameters, commandType: CommandType.StoredProcedure);

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

        public async Task<UserWithRolesDto> UserRolesCheckAsync(string userName)
        {
            try
            {
                using (var connection = _dapperContext.GetDbConnection())
                {
                    connection.Open();

                    // Create DynamicParameters for the stored procedure
                    var parameters = new DynamicParameters();
                    parameters.Add("UserName", userName);

                    // Execute the stored procedure and retrieve roles
                    var roles = await connection.QueryAsync<string>("sp_UserRolesCheck", parameters, commandType: CommandType.StoredProcedure);

                    // Access the roles from the returned UserWithRolesDto
                    var rolesList = roles.ToList(); // Convert to a list if needed

                    // Create UserWithRolesDto
                    var userWithRolesDto = new UserWithRolesDto
                    {
                        UserName = userName,
                        Roles = rolesList
                    };

                    return userWithRolesDto;
                }
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions
                _logger.LogError($"Unexpected error while retrieving user roles: {ex.Message}");
                // Rethrow the exception or handle it as appropriate for your application
                throw;
            }
        }
    }
}
