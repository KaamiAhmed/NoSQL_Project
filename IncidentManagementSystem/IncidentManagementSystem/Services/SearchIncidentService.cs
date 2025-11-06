using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using IncidentManagementSystem.Services.Interfaces;

namespace IncidentManagementSystem.Services
{
    public class SearchIncidentService : ISearchIncidentService
    {
        private readonly ISearchIncidentRepository _searchIncidentRepository;
        public SearchIncidentService(ISearchIncidentRepository searchIncidentRepository)
        {
            _searchIncidentRepository = searchIncidentRepository;
        }
        public async Task<List<Ticket>> SearchIncidentsAsync(string searchWord)
        {
            return await _searchIncidentRepository.SearchIncidentsAsync(searchWord);
        }
    }
}
