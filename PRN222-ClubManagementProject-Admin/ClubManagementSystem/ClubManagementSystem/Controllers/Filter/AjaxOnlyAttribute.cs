using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ClubManagementSystem.Controllers.Filter
{
    public class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                context.Result = new ForbidResult(); // Block normal URL access
            }

            base.OnActionExecuting(context);
        }
    }
}
