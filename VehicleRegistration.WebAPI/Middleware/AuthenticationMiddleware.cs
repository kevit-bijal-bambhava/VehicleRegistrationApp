using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VehicleRegistration.WebAPI.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var controller = context.Request.RouteValues["controller"].ToString();
            if (controller.Equals("Account"))
            {
                await _next(context);
                return;
            }

            var user = context.User;
            if (user.Identity?.IsAuthenticated != true)
            {
                // If the user is not authenticated, returning 401 Unauthorized
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            if (user.Identity?.IsAuthenticated == true)
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                await _next(context);
                return;
            }

            var hasRequiredClaim = user.Claims.Any(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "ExpectedUserId");

            if (!hasRequiredClaim)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden");
                return;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
