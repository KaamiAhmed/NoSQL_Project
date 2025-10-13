using IncidentManagementSystem.Models;
using IncidentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
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
                return RedirectToAction("TicketOverview", "Ticket");
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

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            try
            {
                // build base link
                string? resetLink = Url.Action("ResetPassword", "Home", null, Request.Scheme);
                if (resetLink != null)
                {
                    _userService.ForgotPassword(email, resetLink);
                }

                ViewBag.EmailSent = $"Email with reset password link has been sent to you. Take me to <a href='https://mail.google.com'>Gmail</a>";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.EmailNotSent = $"{ex.Message}";
                return View();
            }
        }

        public IActionResult ResetPassword(string token)
        {
            User user = _userService.CheckTokenValidity(token);
            if (user == null)
            {
                return Content("Invalid Request");
            }

            return View(user.ResetPasswordViewModel);
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            User user = _userService.CheckTokenValidity(resetPasswordViewModel.Token);
            if (user == null)
            {
                return Content("Invalid Request");
            }

            if (resetPasswordViewModel.NewPassword != resetPasswordViewModel.ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match!";
                return View(user.ResetPasswordViewModel);
            }

            //hash password and change it in db and unset the token and expiry fields
            _userService.ResetPassword(user.ResetPasswordViewModel.Token, resetPasswordViewModel.NewPassword);

            TempData["PasswordChanged"] = "Password updated successfully";
            return RedirectToAction("Login");
        }
    }
}
