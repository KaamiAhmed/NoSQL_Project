using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using IncidentManagementSystem.Services.Interfaces;
using MongoDB.Driver;

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
        // Service desk Employee
        public async Task<List<Ticket>> GetAllIncidentsAsync()
        {
            return await _ticketRepository.GetAllIncidentsAsync();
        }

        public async Task<Ticket?> GetIncidentByIdAsync(string id)
        {
            return await _ticketRepository.GetIncidentByIdAsync(id);
        }

        public async Task AddIncidentAsync(Ticket ticket)
        {
            await _ticketRepository.AddIncidentAsync(ticket);
        }

        public async Task UpdateIncidentAsync(Ticket ticket)
        {
            await _ticketRepository.UpdateIncidentAsync(ticket);
        }

        public async Task UpdateIncidentStatusAsync(string id, Status status)
        {          
            await _ticketRepository.UpdateIncidentStatusAsync(id, status);
        }

        // Dashboard Service Desk Employee
        public async Task<long> GetAllTicketsAsync()
        {
            return await _ticketRepository.GetAllTicketsAsync();
        }

        public async Task<long> GetAllOpenTicketsAsync(Status status)
        {
            return await _ticketRepository.GetAllOpenTicketsAsync(status);
        }
        public async Task<long> GetAllResolvedTicketsAsync(Status status)
        {
            return await _ticketRepository.GetAllResolvedTicketsAsync(status);
        }
        public async Task<long> GetAllClosedTicketsAsync(Status status)
        {
            return await _ticketRepository.GetAllClosedTicketsAsync(status);
        }
        public async Task UnassignTicketsFromUserAsync(string userId)
        {
            await _ticketRepository.UnassignTicketsFromUserAsync(userId);
        }
        public async Task<int> CountAssignedToAsync(string userId)
        {
            return await _ticketRepository.CountAssignedToAsync(userId);
        }


    }
}
