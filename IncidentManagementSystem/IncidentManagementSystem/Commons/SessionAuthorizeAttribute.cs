using IncidentManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace IncidentManagementSystem.Commons
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly Role[] _requiredRole;

        public SessionAuthorizeAttribute(params Role[] requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if user is logged in
            User? loggedInUser = null;
            string? userJson = context.HttpContext.Session.GetString("LoggedInUser");
            if (userJson != null)
            {
                loggedInUser = JsonSerializer.Deserialize<User>(userJson);
            }
            if (loggedInUser == null || !_requiredRole.Contains(loggedInUser.Role))
            {
                var controller = (Controller)context.Controller;
                controller.TempData["ErrorMessage"] = "You do not have permission to access this page.";
                context.HttpContext.Session.Remove("LoggedInUser");
                context.Result = new RedirectToActionResult("Login", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
