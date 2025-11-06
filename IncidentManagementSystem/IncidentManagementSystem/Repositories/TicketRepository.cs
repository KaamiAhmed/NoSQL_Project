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
                return await _tickets.Find(t => t.ReportedBy == id).ToListAsync();
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
                var reporter = users.FirstOrDefault(u => u.Id == ticket.ReportedBy);
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
            if (ticket.AssignedTo != null && string.IsNullOrWhiteSpace(ticket.AssignedTo.UserId))
            {
                ticket.AssignedTo = null;
            }

            await _tickets.InsertOneAsync(ticket);
        }
        

        public async Task UpdateIncidentAsync(Ticket ticket)
        {

            var filter = Builders<Ticket>.Filter.Eq(t => t.Id, ticket.Id);
            var update = Builders<Ticket>.Update.Set(t => t.AssignedTo, ticket.AssignedTo).Set(t => t.Priority, ticket.Priority).Set(t => t.Deadline, ticket.Deadline);

            await _tickets.UpdateOneAsync(filter, update);

        }

        public async Task UpdateIncidentStatusAsync(string id, Status status)
        {
            var ticket = await _tickets.Find(t => t.Id == id).FirstOrDefaultAsync();
            if (ticket == null)
                return;

            //if (ticket.AssignedTo != null && string.IsNullOrWhiteSpace(ticket.AssignedTo.UserId))
            //{
            //    ticket.AssignedTo = null;
            //}

           
            ticket.Status = status;

           
            await _tickets.ReplaceOneAsync(t => t.Id == id, ticket);

        }

        // Service Desk Dashbord
        public async Task<long> GetAllTicketsAsync()
        {
            try
            {
                return await _tickets.CountDocumentsAsync(Builders<Ticket>.Filter.Empty);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<long> GetAllOpenTicketsAsync(Status status)
        {
            try
            {
                return await _tickets.CountDocumentsAsync(Builders<Ticket>.Filter.Eq(t => t.Status, Status.Open));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<long> GetAllResolvedTicketsAsync(Status status)
        {
            try
            {
                return await _tickets.CountDocumentsAsync(Builders<Ticket>.Filter.Eq(t => t.Status, Status.Resolved));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<long> GetAllClosedTicketsAsync(Status status)
        {
            try
            {
                return await _tickets.CountDocumentsAsync(Builders<Ticket>.Filter.Eq(t => t.Status, Status.Closed));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
