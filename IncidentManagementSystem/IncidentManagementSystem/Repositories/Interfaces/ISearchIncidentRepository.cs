using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Repositories.Interfaces
{
    public interface ISearchIncidentRepository
    {
        Task<List<Ticket>> SearchIncidentsAsync(string searchWord);
    }
}
