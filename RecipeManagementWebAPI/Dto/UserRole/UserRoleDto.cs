﻿namespace RecipeManagementWebAPI.Dto.UserRole
{
    public class UserRoleDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}
