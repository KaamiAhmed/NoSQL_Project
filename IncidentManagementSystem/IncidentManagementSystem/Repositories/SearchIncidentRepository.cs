using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IncidentManagementSystem.Repositories
{
    public class SearchIncidentRepository : ISearchIncidentRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;


        public SearchIncidentRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Ticket>("Ticket");

        }
        public async Task<List<Ticket>> SearchIncidentsAsync(string searchWord)
        {
            try
            {
                var filter = Builders<Ticket>.Filter.Or(
                    Builders<Ticket>.Filter.Regex(t => t.Title, new BsonRegularExpression(searchWord, "i")),
                    Builders<Ticket>.Filter.Regex(t => t.Description, new BsonRegularExpression(searchWord, "i"))
                );
                // using aggregation to filter & sort by DateOfEvent descending
                return await _tickets.Aggregate().Match(filter).SortByDescending(t => t.DateOfEvent).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
