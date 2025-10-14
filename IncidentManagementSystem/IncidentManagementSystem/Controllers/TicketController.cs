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

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [SessionAuthorize(Role.RegularEmployee)]
        public IActionResult TicketOverview()
        {
            User? loggedInUser = null;
            string? userJson = HttpContext.Session.GetString("LoggedInUser");
            if (userJson != null)
            {
                loggedInUser = JsonSerializer.Deserialize<User>(userJson);
            }
            if (loggedInUser.Role == Role.RegularEmployee)
            {
                ViewData["TotalTickets"] = _ticketService.GetTickets(loggedInUser.Id);

                ViewData["TotalOpenTickets"] = _ticketService.GetOpenTickets(loggedInUser.Id, Status.Open);
                ViewData["TotalResolvedTickets"] = _ticketService.GetResolvedTickets(loggedInUser.Id, Status.Resolved);
                ViewData["TotalClosedTickets"] = _ticketService.GetClosedTickets(loggedInUser.Id, Status.Closed);
            }
            else {
                // viewData all tickets
            }
            ViewData["LoggedInUser"] = loggedInUser;
                return View();
           
        }

        [SessionAuthorize(Role.RegularEmployee)]
        public IActionResult GetMyTickets()
        {
            List<Ticket> myTickets = new List<Ticket>();
            User? loggedInUser = null;
            string? userJson = HttpContext.Session.GetString("LoggedInUser");
            if (userJson != null)
            {
                loggedInUser = JsonSerializer.Deserialize<User>(userJson);
            }

            if (loggedInUser != null)
            {
                if (loggedInUser.Role != Role.RegularEmployee)
                {
                    myTickets = _ticketService.GetTicketsByUser(loggedInUser.Id);
                }
                else {
                    // getall tickets
                }

            }

            ViewData["LoggedInUser"] = loggedInUser;
            return View(myTickets);
        }
    }
}
