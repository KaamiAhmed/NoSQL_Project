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

        public async Task<List<Ticket>> GetTicketsByUserAsync(string id)
        {
            return await _ticketRepository.GetTicketsByUserAsync(id);
        }

        public async Task<long> GetTicketsAsync(string id)
        {
            return await _ticketRepository.GetTicketsAsync(id);
        }

        public async Task<long> GetOpenTicketsAsync(string id, Status status)
        {
            return await _ticketRepository.GetOpenTicketsAsync(id, status);
        }

        public async Task<long> GetResolvedTicketsAsync(string id, Status status)
        {
            return await _ticketRepository.GetResolvedTicketsAsync(id, status);
        }

        public async Task<long> GetClosedTicketsAsync(string id, Status status)
        {
            return await _ticketRepository.GetClosedTicketsAsync(id, status);
        }
    }
}
