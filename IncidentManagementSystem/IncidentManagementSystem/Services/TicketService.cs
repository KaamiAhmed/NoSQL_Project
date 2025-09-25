using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using IncidentManagementSystem.Services.Interfaces;

namespace IncidentManagementSystem.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public List<Ticket> GetTicketsByUser(string id)
        {
            return _ticketRepository.GetTicketsByUser(id);
        }

        public long GetTickets(string id)
        {
            return _ticketRepository.GetTickets(id);
        }

        public long GetOpenTickets(string id, Status status)
        {
            return _ticketRepository.GetOpenTickets(id, status);
        }

        public long GetResolvedTickets(string id, Status status)
        {
            return _ticketRepository.GetResolvedTickets(id, status);
        }

        public long GetClosedTickets(string id, Status status)
        {
            return _ticketRepository.GetClosedTickets(id, status);
        }
    }
}
