using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace VehicleRegistrationMVC.Filters.ActionFilters
{
    public class ValidateJwtTokenFilter : IActionFilter
    {
        private readonly ILogger<ValidateJwtTokenFilter> _logger;

        public ValidateJwtTokenFilter(ILogger<ValidateJwtTokenFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var jwtToken = context.HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(jwtToken))
            {
                _logger.LogInformation("JWT Token is NULL");
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
