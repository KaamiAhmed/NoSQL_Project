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
<<<<<<< HEAD
            var users = _userService.GetAllUsers();
=======
            User? loggedInUser = _userService.GetLoggedInUser();
            ViewData["LoggedInUser"] = loggedInUser;
            var users = await _userService.GetAllUsersAsync();
>>>>>>> 8269353a06638412e9570e569dd161708457849b
            return View(users);
        }
        [SessionAuthorize(Role.Admin)]
        public IActionResult Create()
        {
            ViewBag.Branches = _userService.GetDistinctBranches();
            return View(new User { Name = new Name() }); // ensure nested binding works
        }

<<<<<<< HEAD
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
=======
        //[HttpPost]
        //public IActionResult Create(string name, string email)
        //{
        //    var user = new User { Name = name, Email = email };
        //    _userService.Add(user);
        //    return RedirectToAction("Index");
        //}

        [SessionAuthorize(Role.Admin)]
        public IActionResult Create()
        {
            return View();
>>>>>>> 8269353a06638412e9570e569dd161708457849b
        }

        [HttpPost]
        [SessionAuthorize(Role.Admin)]
<<<<<<< HEAD
        public IActionResult Edit(string id, User updatedUser)
        {
            _userService.UpdateUser(id, updatedUser);
=======
        public async Task<IActionResult> Create(User user)
        {
            await _userService.AddUserAsync(user);
>>>>>>> 8269353a06638412e9570e569dd161708457849b
            return RedirectToAction("Index");
        }

        [SessionAuthorize(Role.Admin)]
<<<<<<< HEAD
        public IActionResult Delete(string id)
        {
            var user = _userService.GetById(id);
=======
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetByIdAsync(id);
>>>>>>> 8269353a06638412e9570e569dd161708457849b
            return View(user);
        }

        [HttpPost]
        [SessionAuthorize(Role.Admin)]
<<<<<<< HEAD
        public IActionResult DeleteConfirmed(string id)
        {
            _userService.DeleteUser(id);
=======
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
>>>>>>> 8269353a06638412e9570e569dd161708457849b
            return RedirectToAction("Index");
        }

    }
}
