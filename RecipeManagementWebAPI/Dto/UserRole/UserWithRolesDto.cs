using RecipeManagementWebAPI.Dto.Role;
using System.ComponentModel;

namespace RecipeManagementWebAPI.Dto.UserRole
{
    public class UserWithRolesDto
    {
       
        public string UserName { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
