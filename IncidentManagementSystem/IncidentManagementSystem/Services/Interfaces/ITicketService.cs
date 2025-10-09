using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Services.Interfaces
{
    public interface ITicketService
    {
        Task<List<Ticket>> GetTicketsByUserAsync(string id);
        Task<long> GetTicketsAsync(string id);
        Task<long> GetOpenTicketsAsync(string id, Status status);
        Task<long> GetResolvedTicketsAsync(string id, Status status);
        Task<long> GetClosedTicketsAsync(string id, Status status);
    }
}
