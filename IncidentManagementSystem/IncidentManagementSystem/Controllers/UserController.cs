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
        public async Task<IActionResult> Create()
        {
            ViewBag.Branches = await _userService.GetDistinctBranchesAsync();
            return View(new User { Name = new Name() }); // ensure nested binding works
        }

        [HttpPost]
        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> Create(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = await _userService.GetDistinctBranchesAsync();
                return View(user);
            }

            try
            {
                await _userService.AddUserAsync(user); 
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
              
                ViewBag.Error = ex.Message;
                ViewBag.Branches = await _userService.GetDistinctBranchesAsync();
                return View(user);
            }
        }


        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null) return RedirectToAction("Index");

            if (user.Name == null) user.Name = new Name();
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
            if (user == null) return RedirectToAction("Index");
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
