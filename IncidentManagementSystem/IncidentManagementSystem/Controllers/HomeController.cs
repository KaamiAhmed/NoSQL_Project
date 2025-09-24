using IncidentManagementSystem.Models;
using IncidentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace IncidentManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login login)
        {

            User? user = _userService.GetUserByLogin(login.UserName, login.Password, login.Role);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Wrong credentials";
                return View(login);
            }
            else
            {
                string jsonUser = JsonSerializer.Serialize(user);
                HttpContext.Session.SetString("LoggedInUser", jsonUser);
                if (login.Role == "regular employee")
                {
                    return RedirectToAction("TicketOverview", "Ticket");
                }
                else
                {
                    return RedirectToAction("Index", "User");
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
