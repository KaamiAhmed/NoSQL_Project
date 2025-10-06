using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;
namespace IncidentManagementSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase db)
        {
            _users = db.GetCollection<User>("User");
        }

        public User GetUserByLogin(string userName, string password, Role role)
        {
            return _users.Find(u => u.UserName == userName && u.Password == password && u.Role == role).FirstOrDefault();
        }

        public List<User> GetAll()
        {
            return _users.Find(_ => true).ToList();
        }

        public void Add(User user)
        {
            _users.InsertOne(user);
        }

        public User EmailAddressExists(string emailAddress)
        {
            return _users.Find(u => u.Email == emailAddress).FirstOrDefault();
        }

        public void AddToken(string email, string token, DateTime tokenExpiry)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var update = Builders<User>.Update.Set(u => u.Token, token).Set(u => u.TokenExpiry, tokenExpiry);
            _users.UpdateOne(filter, update);
        }

        public User CheckTokenValidity(string token)
        {
            return _users.Find(user => user.Token == token &&  user.TokenExpiry > DateTime.UtcNow).FirstOrDefault();
        }

        public void ResetPassword(string token, string password)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Token, token);
            var update = Builders<User>.Update.Set(u => u.Password, password).Unset(u => u.Token).Unset(u => u.TokenExpiry);
            _users.UpdateOne(filter, update);
        }
    }
}
