using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Project.Filters
{
    public class RequireAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var role = http.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                context.Result = new RedirectToActionResult("Login", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
