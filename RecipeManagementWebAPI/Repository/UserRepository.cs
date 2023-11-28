using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeManagementWebAPI.Data;
using RecipeManagementWebAPI.Dto.Role;
using RecipeManagementWebAPI.Dto.User;
using RecipeManagementWebAPI.Dto.UserRole;
using RecipeManagementWebAPI.Models;
using RecipeManagementWebAPI.Services;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RecipeManagementWebAPI.Repository
{
    public class UserRepository:IUserService
    {
        private readonly DapperContext _dapperContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UserRepository> _logger;
        private readonly IConfiguration _configuration;
        public UserRepository(
            DapperContext dapperContext,
            IMapper mapper,
            ILogger<UserRepository> logger,
            IConfiguration configuration
            )
        {
            _dapperContext = dapperContext;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        
        public async Task<bool> UserRegistrationAsync(UserRegistrationDto registrationDto)
        {
            try
            {
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Hash the password before storing it
                    string hashedPassword = HashPassword(registrationDto.Password);

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();
                    parameters.Add("@Username", registrationDto.Username);
                    parameters.Add("@PasswordHash", hashedPassword);
                    parameters.Add("@Email", registrationDto.Email);

                    // Example SQL query for user registration
                    string storedProcedure = "sp_UserRegistration";

                    // Execute the stored procedure
                    var affectedRows = await dbConnection.ExecuteAsync(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    // Log successful registration
                    _logger.LogInformation($"User '{registrationDto.Username}' registered successfully.");

                    // Return true if at least one row was affected (registration successful)
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"Error during user registration: {ex.Message}");
                return false;
            }
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringBuilder.Append(bytes[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }
      
        public async Task<string> LoginUserAsync(string username, string password)
        {
            try
            {
                // Hash the provided password
                var hashedPassword = HashPassword(password);

                // Use Dapper connection for SQL query
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Create DynamicParameters
                    var parameters = new DynamicParameters();
                    parameters.Add("Username", username);
                    parameters.Add("PasswordHash", hashedPassword);
                    parameters.Add("UserId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("RoleName", dbType: DbType.String, size: 20, direction: ParameterDirection.Output);

                    // Execute the stored procedure
                    await dbConnection.ExecuteAsync("sp_LoginUser", parameters, commandType: CommandType.StoredProcedure);

                    // Retrieve output parameters
                    int userId = parameters.Get<int>("UserId");
                    string roleName = parameters.Get<string>("RoleName");

                    if (userId == 0 || string.IsNullOrEmpty(roleName))
                    {
                        _logger.LogWarning("Invalid username or password.");
                        return null; // Invalid credentials
                    }

                    // User is valid, generate a token
                    var token = GenerateToken(userId, roleName);

                    return token;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error authenticating user: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }

        private string GenerateToken(int userId, string userRole)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, userRole),
            // Add any additional claims as needed
        };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(1); // Set your desired expiration time

            var token = new JwtSecurityToken(
                issuer: null, // You can set the issuer if needed
                audience: null, // You can set the audience if needed
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public async Task<bool> UserUpdateAsync(int userId, UpdateUserDto updateUserDto, string oldPassword)
        {
            try
            {
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Verify old password directly in the SQL query
                    var user = await dbConnection.QueryFirstOrDefaultAsync<User>(
                        "SELECT * FROM Users WHERE UserId = @UserId AND PasswordHash = @OldPassword",
                        new { UserId = userId, OldPassword = HashPassword(oldPassword) });

                    if (user == null)
                    {
                        _logger.LogWarning("Invalid old password or user not found.");
                        return false; // Invalid old password or user not found
                    }

                    // Hash the new password
                    string hashedNewPassword = HashPassword(updateUserDto.NewPassword);

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", userId);
                    parameters.Add("@Username", updateUserDto.Username);
                    parameters.Add("@PasswordHash", hashedNewPassword);
                    parameters.Add("@Email", updateUserDto.Email);

                    // Example SQL query for user update
                    string storedProcedure = "sp_UserUpdate";

                    // Execute the stored procedure
                    var affectedRows = await dbConnection.ExecuteAsync(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    // Check if the update was successful
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }
        public async Task<bool> UserDeleteAsync(int userId)
        {
            try
            {
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Check if the user exists before deletion
                    var existingUser = await dbConnection.QueryFirstOrDefaultAsync<User>(
                        "SELECT * FROM Users WHERE UserId = @UserId",
                        new { UserId = userId });

                    if (existingUser == null)
                    {
                        _logger.LogWarning($"User with ID {userId} not found.");
                        return false; // User not found
                    }

                    // Create DynamicParameters
                    var parameters = new DynamicParameters();
                    parameters.Add("UserId", userId);

                    // Execute the stored procedure
                    var affectedRows = await dbConnection.ExecuteAsync("sp_DeleteUser", parameters, commandType: CommandType.StoredProcedure);

                    // Check if the deletion was successful
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting user: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }
    

        public async Task<IEnumerable<UserInfoDto>> GetAllUsersAsync()
        {
            try
            {
                using (IDbConnection dbConnection = _dapperContext.GetDbConnection())
                {
                    dbConnection.Open();

                    // Use dynamic parameters
                    var parameters = new DynamicParameters();

                    // Example SQL query for fetching all users
                    string storedProcedure = "sp_GetAllUsers";

                    // Execute the stored procedure
                    var users = await dbConnection.QueryAsync<UserInfoDto>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return users;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all users: {ex.Message}");
                throw; // Handle the exception based on your application's requirements
            }
        }
    }
}
