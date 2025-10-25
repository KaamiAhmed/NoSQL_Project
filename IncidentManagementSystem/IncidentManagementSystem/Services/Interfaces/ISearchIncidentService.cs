using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Services.Interfaces
{
    public interface ISearchIncidentService
    {
        Task<List<Ticket>> SearchIncidentsAsync(string searchWord);
    }
}
