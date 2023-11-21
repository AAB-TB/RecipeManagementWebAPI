using RecipeManagementWebAPI.Dto.UserRole;

namespace RecipeManagementWebAPI.Services
{
    public interface IUserRoleService
    {
        public Task<UserWithRolesDto> UserRolesCheckAsync(string userName);
        Task<bool> AssignRoleToUserAsync(int userId, int roleId);
        Task<IEnumerable<UserRoleDto>> GetAllUsersWithRolesAsync();
        Task<bool> UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds);
        Task<bool> RemoveUserRolesAsync(int userId, IEnumerable<int> roleIds);
    }
}
