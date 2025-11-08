using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace IncidentManagementSystem.Repositories
{
    public class SearchIncidentRepository : ISearchIncidentRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;
        private readonly IHttpContextAccessor _contextAccessor;


        public SearchIncidentRepository(IMongoDatabase db, IHttpContextAccessor contextAccessor)
        {
            _tickets = db.GetCollection<Ticket>("Ticket");
            _contextAccessor = contextAccessor;

        }

        public async Task<List<Ticket>> SearchIncidentsAsync(string searchWord)
        {
            try
            {
                User? loggedInUser = null;
                string? userJson = _contextAccessor.HttpContext?.Session.GetString("LoggedInUser");

                if (userJson != null)
                {
                    loggedInUser = JsonSerializer.Deserialize<User>(userJson);
                }
                var filter = Builders<Ticket>.Filter.Or(
                    Builders<Ticket>.Filter.Regex(t => t.Title, new BsonRegularExpression(searchWord, "i")),
                    Builders<Ticket>.Filter.Regex(t => t.Description, new BsonRegularExpression(searchWord, "i"))
                );
                if (loggedInUser != null && loggedInUser.Role == Role.RegularEmployee)
                {
                    var userFilter = Builders<Ticket>.Filter.Eq(t => t.ReportedBy, loggedInUser.Id);
                    filter = Builders<Ticket>.Filter.And(filter, userFilter);
                }
                return await _tickets.Aggregate().Match(filter).SortByDescending(t => t.DateOfEvent).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
