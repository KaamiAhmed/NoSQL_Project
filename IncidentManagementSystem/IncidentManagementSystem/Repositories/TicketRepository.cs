using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using MongoDB.Driver;

namespace IncidentManagementSystem.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;
        private readonly IMongoCollection<User> _users;

        public TicketRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Ticket>("Ticket");
            _users = db.GetCollection<User>("User");
        }

        public async Task<List<Ticket>> GetTicketsByUserAsync(string id)
        {
            try
            {
                return await _tickets.Find(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id)).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<long> GetTicketsAsync(string id)
        {
            try
            {
                return await _tickets.CountDocumentsAsync(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<long> GetOpenTicketsAsync(string id, Status status)
        {
            try
            {
                return await _tickets.CountDocumentsAsync(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id) & Builders<Ticket>.Filter.Eq(t => t.Status, Status.Open));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<long> GetResolvedTicketsAsync(string id, Status status)
        {
            try
            {
                return await _tickets.CountDocumentsAsync(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id) & Builders<Ticket>.Filter.Eq(t => t.Status, Status.Resolved));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<long> GetClosedTicketsAsync(string id, Status status)
        {
            try
            {
                return await _tickets.CountDocumentsAsync(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id) & Builders<Ticket>.Filter.Eq(t => t.Status, Status.Closed));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Service desk Employee
        public async Task<List<Ticket>> GetAllIncidentsAsync()
        {
            var tickets = await _tickets.Find(_ => true).ToListAsync();
            var users = await _users.Find(_ => true).ToListAsync();

            foreach (var ticket in tickets)
            {
                var reporter = users.FirstOrDefault(t => t.Id == ticket.ReportedBy);
                ticket.ReportedByName = reporter != null
                    ? $"{reporter.Name.FirstName} {reporter.Name.LastName}"   // if else short version
                    : "Unknown";
            }

            return tickets;
        }


        public async Task<Ticket?> GetIncidentByIdAsync(string id)
        {
            return await _tickets.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddIncidentAsync(Ticket ticket)
        {
            await _tickets.InsertOneAsync(ticket);
        }

        public async Task UpdateIncidentAsync(Ticket ticket)
        {
            var filter = Builders<Ticket>.Filter.Eq(t => t.Id, ticket.Id);
            var update = Builders<Ticket>.Update.Set(t => t.Deadline, ticket.Deadline).Set(t => t.Priority, ticket.Priority);

            await _tickets.UpdateOneAsync(filter, update);

        }

        public async Task UpdateIncidentStatusAsync(string id, Status status)
        {
            var filter = Builders<Ticket>.Filter.Eq(t => t.Id, id);
            var update = Builders<Ticket>.Update.Set(t => t.Status, status);

            await _tickets.UpdateOneAsync(filter, update);

        }

    }
}
