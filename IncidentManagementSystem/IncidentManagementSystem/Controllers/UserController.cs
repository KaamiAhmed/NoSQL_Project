using IncidentManagementSystem.Commons;
using IncidentManagementSystem.Models;
using IncidentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IncidentManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [SessionAuthorize(Role.ServiceDeskEmployee, Role.Admin)]
        public IActionResult Index()
        {
            var users = _userService.GetAll();
            return View(users);
        }

        [HttpPost]
        public IActionResult Create(string name, string email)
        {
            //var user = new User { Name = name, Email = email };
            //_userService.Add(user);
            return RedirectToAction("Index");
        }

    }
}
