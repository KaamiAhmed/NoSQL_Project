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
                    if(myTickets == null || myTickets.Count == 0)
                    {
                        ViewBag.NoTickets = "There are no incidents reported.";
                    }
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
        public IActionResult CreateIncident()
        {
            return View();
        }

        [SessionAuthorize(Role.ServiceDeskEmployee)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateIncident(Ticket model)
        {
            if (!ModelState.IsValid)
                return View(model);
            User? loggedInUser = _userService.GetLoggedInUser();
            model.ReportedBy = loggedInUser.Id;


            model.Status = Status.Open;
            model.DateOfEvent = DateTime.UtcNow;

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
    }
}
