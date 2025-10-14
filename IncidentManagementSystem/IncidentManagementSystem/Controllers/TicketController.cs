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
                myTickets = await _ticketService.GetTicketsByUserAsync(loggedInUser.Id);
            }

            ViewData["LoggedInUser"] = loggedInUser;
            return View(myTickets);
        }
    }
}
