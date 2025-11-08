using IncidentManagementSystem.Commons;
using IncidentManagementSystem.Models;
using IncidentManagementSystem.Services;
using IncidentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IncidentManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITicketService _ticketService;
        public UserController(IUserService userService, ITicketService ticketService)
        {
            _userService = userService;
            _ticketService = ticketService;
        }

        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> Index()
        {
            try
            {
                User? loggedInUser = _userService.GetLoggedInUser();
                ViewData["LoggedInUser"] = loggedInUser;

                var users = await _userService.GetAllUsersAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error while loading users: " + ex.Message;
                return View(new List<User>());
            }
        }

        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Branches = await _userService.GetDistinctBranchesAsync();
                return View(new User { Name = new Name() }); // ensure nested binding works
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error while preparing create page: " + ex.Message;
                return View(new User { Name = new Name() });
            }
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
            try
            {
                var user = await _userService.GetByIdAsync(id);

                if (user == null) return RedirectToAction("Index");

                if (user.Name == null) user.Name = new Name();
                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error while loading edit page: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> Edit(string id, User updatedUser)
        {
            try
            {
                await _userService.UpdateUserAsync(id, updatedUser);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

                ViewBag.Branches = await _userService.GetDistinctBranchesAsync();
                updatedUser.Name ??= new Name();
                return View(updatedUser);
            }
        }

        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return RedirectToAction("Index");

                ViewBag.AssignedCount = await _ticketService.CountAssignedToAsync(id);
                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error while loading delete page: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [SessionAuthorize(Role.Admin)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                TempData["Success"] = "User deleted successfully. All assigned tickets were unassigned.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error while deleting user: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
