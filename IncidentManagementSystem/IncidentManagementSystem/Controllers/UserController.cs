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
            var users = _userService.GetAllUsers();
            return View(users);
        }
        [SessionAuthorize(Role.Admin)]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [SessionAuthorize(Role.Admin)]
        public IActionResult Create(User user)
        {
            _userService.AddUser(user);
            return RedirectToAction("Index");
        }

        [SessionAuthorize(Role.Admin)]
        public IActionResult Edit(string id)
        {
            var user = _userService.GetById(id);
            return View(user);
        }

        [HttpPost]
        [SessionAuthorize(Role.Admin)]
        public IActionResult Edit(string id, User updatedUser)
        {
            _userService.UpdateUser(id, updatedUser);
            return RedirectToAction("Index");
        }

        [SessionAuthorize(Role.Admin)]
        public IActionResult Delete(string id)
        {
            var user = _userService.GetById(id);
            return View(user);
        }

        [HttpPost]
        [SessionAuthorize(Role.Admin)]
        public IActionResult DeleteConfirmed(string id)
        {
            _userService.DeleteUser(id);
            return RedirectToAction("Index");
        }

    }
}
