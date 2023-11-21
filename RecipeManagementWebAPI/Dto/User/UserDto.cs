namespace RecipeManagementWebAPI.Dto.User
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
    }
}
