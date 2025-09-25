using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Services.Interfaces
{
    public interface ITicketService
    {
        List<Ticket> GetTicketsByUser(string id);
        long GetTickets(string id);
        long GetOpenTickets(string id, Status status);
        long GetResolvedTickets(string id, Status status);
        long GetClosedTickets(string id, Status status);
    }
}
