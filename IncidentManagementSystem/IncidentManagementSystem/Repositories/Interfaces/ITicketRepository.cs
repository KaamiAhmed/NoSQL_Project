using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task<List<Ticket>> GetTicketsByUserAsync(string id);
        Task<long> GetTicketsAsync(string id);
        Task<long> GetOpenTicketsAsync(string id, Status status);
        Task<long> GetResolvedTicketsAsync(string id, Status status);
        Task<long> GetClosedTicketsAsync(string id, Status status);

        // Service Desk Employee
         Task<List<Ticket>> GetAllIncidentsAsync();
         Task<Ticket?> GetIncidentByIdAsync(string id);
         Task AddIncidentAsync(Ticket ticket);
         Task UpdateIncidentAsync(Ticket ticket);
         Task UpdateIncidentStatusAsync(string id, Status status);
    }

}
