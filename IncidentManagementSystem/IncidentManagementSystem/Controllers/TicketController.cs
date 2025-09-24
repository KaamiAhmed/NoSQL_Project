using IncidentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IncidentManagementSystem.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult TicketOverview()
        {
            User? loggedInUser = null;
            string? userJson = HttpContext.Session.GetString("LoggedInUser");
            if (userJson != null)
            {
                loggedInUser = JsonSerializer.Deserialize<User>(userJson);
            }

            ViewData["LoggedInUser"] = loggedInUser;
            return View();
        }
    }
}
