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

        public List<Ticket> GetTicketsByUser(string id)
        {
            return _tickets.Find(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id)).ToList();
        }

        public long GetTickets(string id)
        {
            return _tickets.CountDocuments(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id));
        }

        public long GetOpenTickets(string id, Status status)
        {
            return _tickets.CountDocuments(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id) & Builders<Ticket>.Filter.Eq(t => t.Status, Status.Open));
        }

        public long GetResolvedTickets(string id, Status status)
        {
            return _tickets.CountDocuments(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id) & Builders<Ticket>.Filter.Eq(t => t.Status, Status.Resolved));
        }

        public long GetClosedTickets(string id, Status status)
        {
            return _tickets.CountDocuments(Builders<Ticket>.Filter.Eq(t => t.ReportedBy, id) & Builders<Ticket>.Filter.Eq(t => t.Status, Status.Closed));
        }
    }
}
