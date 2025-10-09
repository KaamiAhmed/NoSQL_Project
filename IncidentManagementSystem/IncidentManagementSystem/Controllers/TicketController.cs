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
        public async Task<IActionResult> TicketOverview()
        {
            User? loggedInUser = null;
            string? userJson = HttpContext.Session.GetString("LoggedInUser");
            if (userJson != null)
            {
                loggedInUser = JsonSerializer.Deserialize<User>(userJson);
            }

            ViewData["TotalTickets"] = await _ticketService.GetTicketsAsync(loggedInUser.Id);

            ViewData["TotalOpenTickets"] = await _ticketService.GetOpenTicketsAsync(loggedInUser.Id, Status.Open);
            ViewData["TotalResolvedTickets"] = await _ticketService.GetResolvedTicketsAsync(loggedInUser.Id, Status.Resolved);
            ViewData["TotalClosedTickets"] = await _ticketService.GetClosedTicketsAsync(loggedInUser.Id, Status.Closed);

            ViewData["LoggedInUser"] = loggedInUser;
            return View();
        }

        [SessionAuthorize(Role.RegularEmployee)]
        public async Task<IActionResult> GetMyTickets()
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
                myTickets = await _ticketService.GetTicketsByUserAsync(loggedInUser.Id);
            }

            ViewData["LoggedInUser"] = loggedInUser;
            return View(myTickets);
        }
    }
}
