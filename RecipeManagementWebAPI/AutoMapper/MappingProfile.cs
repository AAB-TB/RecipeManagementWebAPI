using AutoMapper;
using RecipeManagementWebAPI.Dto.Category;
using RecipeManagementWebAPI.Dto.Rating;
using RecipeManagementWebAPI.Dto.Recipe;
using RecipeManagementWebAPI.Dto.Role;
using RecipeManagementWebAPI.Dto.User;
using RecipeManagementWebAPI.Dto.UserRole;
using RecipeManagementWebAPI.Models;

namespace RecipeManagementWebAPI.AutoMapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            //mapper.Map<DestinationType>(sourceObject)

            // Model to DTO mappings
            CreateMap<User, UserDto>();  //<source,destination>
            CreateMap<User, UserInfoDto>();
            CreateMap<Role, RoleDto>();


        }
    }
}
