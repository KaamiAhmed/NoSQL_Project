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
            ViewBag.Branches = _userService.GetDistinctBranches();
            return View(new User { Name = new Name() }); // ensure nested binding works
        }

        [HttpPost]
        [SessionAuthorize(Role.Admin)]
        public IActionResult Create(User user)
        {
            try
            {
                _userService.AddUser(user); 
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
              
                ViewBag.Error = ex.Message;
                return View(user);
            }
        }


        [SessionAuthorize(Role.Admin)]
        public IActionResult Edit(string id)
        {
            var user = _userService.GetById(id);
            if (user == null) return RedirectToAction("Index");

            if (user.Name == null) user.Name = new Name(); // so nested binding works
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
