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
    }
}
