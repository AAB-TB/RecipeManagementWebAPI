using RecipeManagementWebAPI.Dto.Role;


namespace RecipeManagementWebAPI.Services
{
    public interface IRoleService
    {
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        
    }
}
