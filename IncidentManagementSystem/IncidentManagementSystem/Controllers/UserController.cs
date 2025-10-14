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

        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> Index()
        {

            User? loggedInUser = _userService.GetLoggedInUser();
            ViewData["LoggedInUser"] = loggedInUser;
            var users = await _userService.GetAllUsersAsync();
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

        //[HttpPost]
        //public IActionResult Create(string name, string email)
        //{
        //    var user = new User { Name = name, Email = email };
        //    _userService.Add(user);
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> Create(User user)
        {
            await _userService.AddUserAsync(user);
            return RedirectToAction("Index");
        }

        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> Edit(string id, User updatedUser)
        {
            await _userService.UpdateUserAsync(id, updatedUser);
            return RedirectToAction("Index");
        }

        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction("Index");
        }

    }
}
