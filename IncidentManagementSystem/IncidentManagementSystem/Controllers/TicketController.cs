using IncidentManagementSystem.Commons;
using IncidentManagementSystem.Models;
using IncidentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IncidentManagementSystem.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;

        public TicketController(ITicketService ticketService, IUserService userService)
        {
            _ticketService = ticketService;
            _userService = userService;
        }

        [SessionAuthorize(Role.RegularEmployee, Role.ServiceDeskEmployee, Role.Admin)]
        public async Task<IActionResult> TicketOverview()
        {
            User? loggedInUser = _userService.GetLoggedInUser();

            ViewData["TotalTickets"] = await _ticketService.GetTicketsAsync(loggedInUser.Id);

            ViewData["TotalOpenTickets"] = await _ticketService.GetOpenTicketsAsync(loggedInUser.Id, Status.Open);
            ViewData["TotalResolvedTickets"] = await _ticketService.GetResolvedTicketsAsync(loggedInUser.Id, Status.Resolved);
            ViewData["TotalClosedTickets"] = await _ticketService.GetClosedTicketsAsync(loggedInUser.Id, Status.Closed);

            ViewData["LoggedInUser"] = loggedInUser;
            return View();
        }

        [SessionAuthorize(Role.RegularEmployee, Role.ServiceDeskEmployee, Role.Admin)]
        public async Task<IActionResult> GetMyTickets()
        {
            User? loggedInUser = _userService.GetLoggedInUser();
            List<Ticket> myTickets = new List<Ticket>();
            if (loggedInUser != null)
            {
                if (loggedInUser.Role == Role.ServiceDeskEmployee)
                {
                    myTickets = await _ticketService.GetAllIncidentsAsync();
                }
                else
                {
                    myTickets = await _ticketService.GetTicketsByUserAsync(loggedInUser.Id);
                }
                ViewData["LoggedInUser"] = loggedInUser;
                ViewBag.UserRole = loggedInUser.Role.ToString();
            }
            return View(myTickets);
        }



        [SessionAuthorize(Role.ServiceDeskEmployee)]
        public async Task<IActionResult> GetAllIncidents()
        {
            User? loggedInUser = _userService.GetLoggedInUser();
            ViewBag.UserRole = loggedInUser.Role.ToString();
            var allTickets = await _ticketService.GetAllIncidentsAsync();
            return View("GetMyTickets", allTickets);
        }

        [SessionAuthorize(Role.ServiceDeskEmployee)]
        [HttpGet]
        public async Task<IActionResult> CreateIncident()
        {
            var regularEmployees = await _userService.GetUsersByRoleAsync(Role.RegularEmployee) ?? new List<User>();
            ViewBag.RegularEmployees = regularEmployees;
            ViewBag.LoggedInUser = _userService.GetLoggedInUser();
            return View();
        }

        [SessionAuthorize(Role.ServiceDeskEmployee)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateIncident(Ticket model)
        {
            var loggedInUser = _userService.GetLoggedInUser();
            ViewBag.LoggedInUser = loggedInUser;
            ViewBag.RegularEmployees = await _userService.GetUsersByRoleAsync(Role.RegularEmployee);

            if (!ModelState.IsValid)
                return View(model);

            model.Status = Status.Open;
            model.DateOfEvent = DateTime.UtcNow;

            // Auto-assign for Regular Employees
            if (loggedInUser.Role == Role.RegularEmployee)
            {
                model.ReportedBy = loggedInUser.Id;
            }

            switch (model.Priority)
            {
                case Priority.High:
                    model.Deadline = 7;
                    break;
                case Priority.Normal:
                    model.Deadline = 14;
                    break;
                case Priority.Low:
                    model.Deadline = 21;
                    break;
            }

            await _ticketService.AddIncidentAsync(model);
            TempData["SuccessMessage"] = "Incident created successfully!";
            return RedirectToAction("GetMyTickets", "Ticket");
        }
        [SessionAuthorize(Role.ServiceDeskEmployee)]
        [HttpGet]
        public async Task<IActionResult> UpdateIncident(string id)
        {
            var ticket = await _ticketService.GetIncidentByIdAsync(id);
            if (ticket == null)
                return NotFound();
            if (!string.IsNullOrEmpty(ticket.ReportedBy))
            {
                var reporter = await _userService.GetByIdAsync(ticket.ReportedBy);
                if (reporter != null)
                {
                    ticket.ReportedByName = $"{reporter.Name.FirstName} {reporter.Name.LastName}";
                }
            }

            var serviceDeskEmployees = await _userService.GetUsersByRoleAsync(Role.ServiceDeskEmployee);
            ViewBag.ServiceDeskEmployees = serviceDeskEmployees;
            ViewData["LoggedInUser"] = _userService.GetLoggedInUser();
            return View(ticket);
        }

        [SessionAuthorize(Role.ServiceDeskEmployee)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIncident(Ticket model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var selectedEmp = await _userService.GetByIdAsync(model.AssignedTo.UserId);
            if (selectedEmp != null)
            {
                model.AssignedTo.FirstName = selectedEmp.Name.FirstName;
                model.AssignedTo.LastName = selectedEmp.Name.LastName;
            }


            await _ticketService.UpdateIncidentAsync(model);
            TempData["SuccessMessage"] = "Incident updated successfully!";
            return RedirectToAction("GetMyTickets", "Ticket");
        }
        [SessionAuthorize(Role.ServiceDeskEmployee)]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string id, Status status)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid ticket ID");

            await _ticketService.UpdateIncidentStatusAsync(id, status);
            return Json(new { success = true, newStatus = status.ToString() });
        }


    }

}
