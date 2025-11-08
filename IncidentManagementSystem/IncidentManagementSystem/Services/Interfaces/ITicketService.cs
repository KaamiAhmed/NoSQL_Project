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

        //Service Desk 
        Task<List<Ticket>> GetAllIncidentsAsync();
        Task<Ticket?> GetIncidentByIdAsync(string id);
        Task AddIncidentAsync(Ticket ticket);
        Task UpdateIncidentAsync(Ticket ticket);
        Task UpdateIncidentStatusAsync(string id, Status status);

        // Dashboard Service Desk Employee
        Task<long> GetAllTicketsAsync();
        Task<long> GetAllOpenTicketsAsync(Status status);
        Task<long> GetAllResolvedTicketsAsync(Status status);
        Task<long> GetAllClosedTicketsAsync(Status status);
        // Admin
        Task UnassignTicketsFromUserAsync(string userId);
        Task<int> CountAssignedToAsync(string userId);


    }
}
