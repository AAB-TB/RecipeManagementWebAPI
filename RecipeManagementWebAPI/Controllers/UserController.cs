using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementWebAPI.Dto.Login;
using RecipeManagementWebAPI.Dto.User;
using RecipeManagementWebAPI.Services;

namespace RecipeManagementWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(
            IUserService userService,
            ILogger<UserController> logger
            )
        {
            _userService = userService;
            _logger = logger;
        }


        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Invalid input. Please provide a username and password.");
            }

            var token = await _userService.LoginUserAsync(loginRequest.Username, loginRequest.Password);

            if (token == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(new { Token = token });
        }

        [HttpPost("Register")]
        public async Task<ActionResult<bool>> RegisterUser([FromBody] UserRegistrationDto registrationDto)
        {
            try
            {
                var isRegistered = await _userService.UserRegistrationAsync(registrationDto);

                if (isRegistered)
                {
                    return Ok(true);
                }
                else
                {
                    // You can customize the response based on your requirements
                    return BadRequest("User registration failed. Check the provided data and try again.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during user registration: {ex.Message}");
                return StatusCode(500, "Unexpected error during user registration.");
            }
        }

        [HttpPut("UpdateUser/{userId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<bool>> UpdateUser(int userId, [FromBody] UpdateUserDto updateUserDto, string oldPassword)
        {
            try
            {
                var isUpdated = await _userService.UserUpdateAsync(userId, updateUserDto, oldPassword);

                if (isUpdated)
                {
                    return Ok(true);
                }
                else
                {
                    // You can customize the response based on your requirements
                    return NotFound($"User with ID {userId} not found or update failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during user update: {ex.Message}");
                return StatusCode(500, "Unexpected error during user update.");
            }
        }

        [HttpGet("allUsers")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                if (users != null && users.Any())
                {
                    return Ok(users);
                }
                else
                {
                    return NotFound("No users found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all users: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpDelete("DeleteUser/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteUser(int userId)
        {
            try
            {
                var isDeleted = await _userService.UserDeleteAsync(userId);

                if (isDeleted)
                {
                    return Ok(true);
                }
                else
                {
                    // You can customize the response based on your requirements
                    return NotFound($"User with ID {userId} not found or delete failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during user delete: {ex.Message}");
                return StatusCode(500, "Unexpected error during user delete.");
            }
        }

    }
}
