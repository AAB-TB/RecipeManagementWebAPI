using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeManagementWebAPI.Dto.Role;
using RecipeManagementWebAPI.Services;

namespace RecipeManagementWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;
        public RoleController(
            IRoleService roleService,
            ILogger<RoleController> logger
            )
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpPost("CreateRole")]
        
        public async Task<ActionResult<bool>> CreateRole([FromBody] RoleDto roleDto)
        {
            try
            {
                var isCreated = await _roleService.CreateRoleAsync(roleDto.RoleName);

                if (isCreated)
                {
                    return Ok(true);
                }
                else
                {
                    // You can customize the response based on your requirements
                    return BadRequest($"Role '{roleDto.RoleName}' already exists or creation failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during role creation: {ex.Message}");
                return StatusCode(500, "Unexpected error during role creation.");
            }
        }
        [HttpDelete("DeleteRole/{roleId}")]
        
        public async Task<ActionResult<bool>> DeleteRole(int roleId)
        {
            try
            {
                var isDeleted = await _roleService.DeleteRoleAsync(roleId);

                if (isDeleted)
                {
                    return Ok(true);
                }
                else
                {
                    // You can customize the response based on your requirements
                    return NotFound($"Role with ID {roleId} not found or delete failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during role deletion: {ex.Message}");
                return StatusCode(500, "Unexpected error during role deletion.");
            }
        }
        [HttpGet("GetAllRoles")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while retrieving all roles: {ex.Message}");
                return StatusCode(500, "Unexpected error while retrieving all roles.");
            }
        }
    }
}
