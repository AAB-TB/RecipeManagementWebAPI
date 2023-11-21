using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementWebAPI.Dto.UserRole;
using RecipeManagementWebAPI.Services;

namespace RecipeManagementWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;
        private readonly ILogger<UserRoleController> _logger;
        public UserRoleController(
            IUserRoleService userRoleService,
            ILogger<UserRoleController> logger
            )
        {
            _userRoleService = userRoleService;
            _logger = logger;
        }

        [HttpPost("AssignRoleToUser")]
       
        public async Task<ActionResult<bool>> AssignRoleToUser([FromBody] AssignRoleDto assignRoleDto)
        {
            try
            {
                var isAssigned = await _userRoleService.AssignRoleToUserAsync(assignRoleDto.UserId, assignRoleDto.RoleId);

                if (isAssigned)
                {
                    return Ok(true);
                }
                else
                {
                    // You can customize the response based on your requirements
                    return BadRequest($"Role with ID {assignRoleDto.RoleId} already assigned to user with ID {assignRoleDto.UserId} or assignment failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during role assignment: {ex.Message}");
                return StatusCode(500, "Unexpected error during role assignment.");
            }
        }
        [HttpGet("GetAllUsersWithRoles")]
       
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetAllUsersWithRoles()
        {
            try
            {
                var usersWithRoles = await _userRoleService.GetAllUsersWithRolesAsync();
                return Ok(usersWithRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching users with roles: {ex.Message}");
                return StatusCode(500, "Unexpected error while fetching users with roles.");
            }
        }
        [HttpPut("UpdateUserRoles/{userId}")]
        
        public async Task<ActionResult<bool>> UpdateUserRoles(int userId, [FromBody] IEnumerable<int> roleIds)
        {
            try
            {
                var isUpdated = await _userRoleService.UpdateUserRolesAsync(userId, roleIds);
                if (isUpdated)
                {
                    return Ok(true);
                }
                else
                {
                    // You can customize the response based on your requirements
                    return BadRequest($"Error updating roles for user with ID {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during user role update: {ex.Message}");
                return StatusCode(500, "Unexpected error during user role update.");
            }
        }
        [HttpPut("RemoveUserRoles/{userId}")]
        
        public async Task<ActionResult<bool>> RemoveUserRoles(int userId, [FromBody] IEnumerable<int> roleIds)
        {
            try
            {
                var isRemoved = await _userRoleService.RemoveUserRolesAsync(userId, roleIds);
                if (isRemoved)
                {
                    return Ok(true);
                }
                else
                {
                    // You can customize the response based on your requirements
                    return BadRequest($"Error removing roles for user with ID {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during user role removal: {ex.Message}");
                return StatusCode(500, "Unexpected error during user role removal.");
            }
        }
    }
}
