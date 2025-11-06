using IncidentManagementSystem.Commons;
using IncidentManagementSystem.Models;
using IncidentManagementSystem.Services;
using IncidentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IncidentManagementSystem.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;
        private readonly ISearchIncidentService _searchIncidentService;

        public TicketController(ITicketService ticketService, IUserService userService, ISearchIncidentService searchIncidentService)
        {
            _ticketService = ticketService;
            _userService = userService;
            _searchIncidentService = searchIncidentService;
        }

        [SessionAuthorize(Role.RegularEmployee, Role.ServiceDeskEmployee, Role.Admin)]
        public async Task<IActionResult> TicketOverview()
        {
            User? loggedInUser = _userService.GetLoggedInUser();
            if (loggedInUser != null && loggedInUser.Role == Role.RegularEmployee)
            {
                ViewData["TotalTickets"] = await _ticketService.GetTicketsAsync(loggedInUser.Id);

                ViewData["TotalOpenTickets"] = await _ticketService.GetOpenTicketsAsync(loggedInUser.Id, Status.Open);
                ViewData["TotalResolvedTickets"] = await _ticketService.GetResolvedTicketsAsync(loggedInUser.Id, Status.Resolved);
                ViewData["TotalClosedTickets"] = await _ticketService.GetClosedTicketsAsync(loggedInUser.Id, Status.Closed);
            }
            else
            {
                ViewData["TotalAllTickets"] = await _ticketService.GetAllTicketsAsync();
                ViewData["TotalAllOpenTickets"] = await _ticketService.GetAllOpenTicketsAsync(Status.Open);
                ViewData["TotalAllResolvedTickets"] = await _ticketService.GetAllResolvedTicketsAsync(Status.Resolved);
                ViewData["TotalAllClosedTickets"] = await _ticketService.GetAllClosedTicketsAsync(Status.Closed);
            }

            ViewData["LoggedInUser"] = loggedInUser;
            return View();

        }

        [SessionAuthorize(Role.RegularEmployee, Role.ServiceDeskEmployee, Role.Admin)]
        public async Task<IActionResult> GetMyTickets(string searchWord)
        {
            User? loggedInUser = _userService.GetLoggedInUser();
            List<Ticket> myTickets = new List<Ticket>();
            if (loggedInUser != null)
            {
                if (loggedInUser.Role == Role.RegularEmployee)
                {
                    myTickets = await _ticketService.GetTicketsByUserAsync(loggedInUser.Id);
                }
                else
                {
                    myTickets = await _ticketService.GetAllIncidentsAsync();
                }

                if (myTickets == null || myTickets.Count == 0)
                {
                    ViewBag.NoTickets = "There are no incidents reported.";
                }

                if (searchWord != null)
                {
                    myTickets = await _searchIncidentService.SearchIncidentsAsync(searchWord);
                }
                ViewData["LoggedInUser"] = loggedInUser;
                ViewData["CurrentFilter"] = searchWord;
                ViewBag.UserRole = loggedInUser.Role.ToString();
            }
            return View(myTickets);
        }



        [SessionAuthorize(Role.ServiceDeskEmployee)]
        public async Task<IActionResult> GetAllIncidents()
        {
            try
            {
                var loggedInUser = _userService.GetLoggedInUser();
                ViewBag.UserRole = loggedInUser.Role.ToString();

                var allTickets = await _ticketService.GetAllIncidentsAsync();
                return View("GetMyTickets", allTickets);
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                Console.Error.WriteLine($"MongoDB Error: {ex.Message}");
                ModelState.AddModelError("", "Database issue occurred. Please try again later.");
                return View("GetMyTickets");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Something went wrong while loading incidents. Please try again.");
                return View("GetMyTickets");
            }
        }


        [SessionAuthorize(Role.ServiceDeskEmployee, Role.RegularEmployee)]
        [HttpGet]
        public async Task<IActionResult> CreateIncident()
        {
            try
            {
                var regularEmployees = await _userService.GetUsersByRoleAsync(Role.RegularEmployee) ?? new List<User>();
                ViewBag.RegularEmployees = regularEmployees;
                ViewBag.LoggedInUser = _userService.GetLoggedInUser();
                return View();
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                Console.Error.WriteLine($"MongoDB Error: {ex.Message}");
                ModelState.AddModelError("", "Database issue occurred. Please try again later.");
                return View();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Something went wrong while loading the form. Please try again.");
                return View();
            }
        }


        [SessionAuthorize(Role.ServiceDeskEmployee, Role.RegularEmployee)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateIncident(Ticket model)
        {
            try
            {
                var loggedInUser = _userService.GetLoggedInUser();
                ViewBag.LoggedInUser = loggedInUser;
                ViewBag.RegularEmployees = await _userService.GetUsersByRoleAsync(Role.RegularEmployee);

                if (!ModelState.IsValid)
                    return View(model);

                model.Status = Status.Open;
                model.DateOfEvent = DateTime.UtcNow;

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
                    default:
                        model.Deadline = null;
                        break;
                }

                await _ticketService.AddIncidentAsync(model);

                TempData["SuccessMessage"] = "Incident created successfully!";
                return RedirectToAction("GetMyTickets", "Ticket");
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                Console.Error.WriteLine($"MongoDB Error: {ex.Message}");
                ModelState.AddModelError("", "Database issue occurred. Please try again later.");
                return View(model);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Something went wrong while creating the incident. Please try again.");
                return View(model);
            }
        }

        [SessionAuthorize(Role.ServiceDeskEmployee)]
        [HttpGet]
        public async Task<IActionResult> UpdateIncident(string id)
        {
            try
            {
                var ticket = await _ticketService.GetIncidentByIdAsync(id);
                if (ticket == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(ticket.ReportedBy))
                {
                    var reporter = await _userService.GetByIdAsync(ticket.ReportedBy);
                    if (reporter != null)
                        ticket.ReportedByName = $"{reporter.Name.FirstName} {reporter.Name.LastName}";
                }

                var serviceDeskEmployees = await _userService.GetUsersByRoleAsync(Role.ServiceDeskEmployee);
                ViewBag.ServiceDeskEmployees = serviceDeskEmployees;
                ViewData["LoggedInUser"] = _userService.GetLoggedInUser();

                return View(ticket);
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                Console.Error.WriteLine($"MongoDB Error: {ex.Message}");
                ModelState.AddModelError("", "Database issue occurred. Please try again later.");
                return View();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Something went wrong while loading the incident. Please try again.");
                return View();
            }
        }


        [SessionAuthorize(Role.ServiceDeskEmployee)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIncident(Ticket model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var selectedEmp = await _userService.GetByIdAsync(model.AssignedTo.UserId);
                if (selectedEmp != null)
                {
                    model.AssignedTo.FirstName = selectedEmp.Name.FirstName;
                    model.AssignedTo.LastName = selectedEmp.Name.LastName;
                }
                // Update deadline based on priority
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

                await _ticketService.UpdateIncidentAsync(model);

                TempData["SuccessMessage"] = "Incident updated successfully!";
                return RedirectToAction("GetMyTickets", "Ticket");
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                Console.Error.WriteLine($"MongoDB Error: {ex.Message}");
                ModelState.AddModelError("", "Database issue occurred. Please try again later.");
                return View(model);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Something went wrong while updating the incident. Please try again.");
                return View(model);
            }
        }

        [SessionAuthorize(Role.ServiceDeskEmployee)]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string id, Status status)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Invalid ticket ID");

                await _ticketService.UpdateIncidentStatusAsync(id, status);
                return Json(new { success = true, newStatus = status.ToString() });
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                Console.Error.WriteLine($"MongoDB Error: {ex.Message}");
                return Json(new { success = false, message = "Database issue occurred. Please try again later." });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "Something went wrong while updating the status. Please try again." });
            }
        }



    }

}
