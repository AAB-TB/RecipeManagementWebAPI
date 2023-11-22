using System.Security.Claims;

namespace RecipeManagementWebAPI.Repository
{
    public class RoleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Exclude the authentication path from role authorization
            if (context.Request.Path.Equals("/api/user/login", StringComparison.OrdinalIgnoreCase) ||
    context.Request.Path.Equals("/api/user/Register", StringComparison.OrdinalIgnoreCase) ||
     context.Request.Path.Equals("/api/user/allUsers", StringComparison.OrdinalIgnoreCase) ||
     context.Request.Path.Equals("/api/role/GetAllRoles", StringComparison.OrdinalIgnoreCase) ||
     context.Request.Path.Equals("/api/recipe/allRecipes", StringComparison.OrdinalIgnoreCase) ||
     context.Request.Path.Equals("/api/recipe/categories", StringComparison.OrdinalIgnoreCase) ||
      context.Request.Path.Equals("/api/recipe/title", StringComparison.OrdinalIgnoreCase) ||
      context.Request.Path.Equals("/api/recipe/search", StringComparison.OrdinalIgnoreCase) ||
      context.Request.Path.Equals("/api/category/allCategories", StringComparison.OrdinalIgnoreCase) ||
    context.Request.Path.Equals("/api/UserRole/GetAllUsersWithRoles", StringComparison.OrdinalIgnoreCase) ||
    context.Request.Path.Equals("/api/UserRole/user-roles", StringComparison.OrdinalIgnoreCase))
            {
                await _next.Invoke(context);
                return;
            }


            var user = context.User;

            // Check if the user has the "Admin" or "Customer" role.
            if (user.HasClaim(c => c.Type == ClaimTypes.Role && (c.Value == "Admin" || c.Value == "Customer")))
            {
                // User has either "Admin" or "Customer" role, allow access to the endpoint.
                await _next.Invoke(context);
            }
            else
            {
                // User doesn't have the required role, return unauthorized.
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
            }
        }
    }

}
