using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using MongoDB.Driver;

namespace IncidentManagementSystem.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;

        public TicketRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Ticket>("Ticket");
        }

        public async Task<List<Ticket>> GetTicketsByUserAsync(string id)
        {
            return await _tickets.Find(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id)).ToListAsync();
        }

        public async Task<long> GetTicketsAsync(string id)
        {
            return await _tickets.CountDocumentsAsync(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id));
        }

        public async Task<long> GetOpenTicketsAsync(string id, Status status)
        {
            return await _tickets.CountDocumentsAsync(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id) & Builders<Ticket>.Filter.Eq(t => t.Status, Status.Open));
        }

        public async Task<long> GetResolvedTicketsAsync(string id, Status status)
        {
            return await _tickets.CountDocumentsAsync(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id) & Builders<Ticket>.Filter.Eq(t => t.Status, Status.Resolved));
        }

        public async Task<long> GetClosedTicketsAsync(string id, Status status)
        {
            return await _tickets.CountDocumentsAsync(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id) & Builders<Ticket>.Filter.Eq(t => t.Status, Status.Closed));
        }
    }
}
