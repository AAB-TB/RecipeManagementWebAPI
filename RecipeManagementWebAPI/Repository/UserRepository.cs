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

        
        private async Task<string> GetUserRoleAsync(string username)
        {
            string sqlQuery = "SELECT r.RoleName FROM UserRoles ur " +
                      "JOIN Roles r ON ur.RoleId = r.RoleId " +
                      "JOIN Users u ON ur.UserId = u.UserId " +
                      "WHERE LOWER(u.Username) = LOWER(@Username)";

            using (var connection = _dapperContext.GetDbConnection())
            {
                var role = await connection.ExecuteScalarAsync<string>(sqlQuery, new { Username = username.ToLower() });
                return role;
            }
        }
        
        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] inputBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));

                // Convert byte array to a string
                StringBuilder inputStringBuilder = new StringBuilder();
                for (int i = 0; i < inputBytes.Length; i++)
                {
                    inputStringBuilder.Append(inputBytes[i].ToString("x2"));
                }

                // Compare the hashed input password with the stored hashed password
                return inputStringBuilder.ToString().Equals(storedHashedPassword);
            }
        }


        public Task<bool> UserDeleteAsync(int userId)
        {
            throw new NotImplementedException();
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

                    // Example SQL query for user registration
                    string sqlQuery = @"
                INSERT INTO Users (Username, PasswordHash, Email)
                VALUES (@Username, @PasswordHash, @Email);";

                    // Execute the query
                    var affectedRows = await dbConnection.ExecuteAsync(sqlQuery, new
                    {
                        registrationDto.Username,
                        PasswordHash = hashedPassword,
                        registrationDto.Email
                    });

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
        public Task<bool> UserUpdateAsync(int userId, UpdateUserDto updateUserDto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserInfoDto>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
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

                    // Modified SQL query with JOIN
                    var user = await dbConnection.QueryFirstOrDefaultAsync<UserIdWithRoleNameDto>(
                        @"SELECT
                        Users.UserId,
                        Users.Username,
                        Users.PasswordHash,
                        Roles.RoleName
                  FROM
                        Users
                  JOIN
                        UserRoles ON Users.UserId = UserRoles.UserId
                  JOIN
                        Roles ON UserRoles.RoleId = Roles.RoleId
                  WHERE
                        LOWER(Users.Username) = LOWER(@Username) AND Users.PasswordHash = @PasswordHash",
                        new { Username = username, PasswordHash = hashedPassword });

                    if (user == null)
                    {
                        _logger.LogWarning("Invalid username or password.");
                        return null; // Invalid credentials
                    }

                    // User is valid, generate a token
                    var token = GenerateToken(user.UserId, user.RoleName);

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
    }
}
