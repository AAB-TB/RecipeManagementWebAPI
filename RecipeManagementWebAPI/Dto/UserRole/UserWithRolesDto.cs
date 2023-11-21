using RecipeManagementWebAPI.Dto.Role;

namespace RecipeManagementWebAPI.Dto.UserRole
{
    public class UserWithRolesDto
    {
        public int UserName { get; set; }
        public List<RoleDto> Roles { get; set; }
    }
}
