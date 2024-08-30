using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace VehicleRegistrationMVC.Filters.ActionFilters
{
    public class ModelStateValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var controller = context.Controller as Controller;
                if (controller != null)
                {
                    var model = context.ActionArguments.Values.FirstOrDefault();
                    context.Result = controller.View(model);
                }
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {   
        }
    }
}
