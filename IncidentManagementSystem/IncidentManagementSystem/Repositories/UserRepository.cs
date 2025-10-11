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
        public User? GetById(string id)
        {
            return _users.Find(u => u.Id == id).FirstOrDefault();
        }

        public List<User> GetAllUsers()
        {
            return _users.Find(_ => true).ToList();
        }

        public void AddUser(User user)
        {
            _users.InsertOne(user);
        }
        public void UpdateUser(string id, User updatedUser)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var update = Builders<User>.Update
                .Set(u => u.UserName, updatedUser.UserName)
                .Set(u => u.Email, updatedUser.Email)
                .Set(u => u.Password, updatedUser.Password)
                .Set(u => u.Role, updatedUser.Role);

            _users.UpdateOne(filter, update);
        }
        public void DeleteUser(string id)
        {
            _users.DeleteOne(u => u.Id == id);
        }

        public User EmailAddressExists(string emailAddress)
        {
            return _users.Find(u => u.Email == emailAddress).FirstOrDefault();
        }

        public void AddToken(string email, string token, DateTime tokenExpiry)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var update = Builders<User>.Update.Set(u => u.ResetPasswordViewModel.Token, token).Set(u => u.ResetPasswordViewModel.TokenExpiry, tokenExpiry);
            _users.UpdateOne(filter, update);
        }

        public User CheckTokenValidity(string token)
        {
            return _users.Find(user => user.ResetPasswordViewModel.Token == token &&  user.ResetPasswordViewModel.TokenExpiry > DateTime.UtcNow).FirstOrDefault();
        }

        public void ResetPassword(string token, string password)
        {
            var filter = Builders<User>.Filter.Eq(u => u.ResetPasswordViewModel.Token, token);
            var update = Builders<User>.Update.Set(u => u.Password, password).Unset(u => u.ResetPasswordViewModel);
            _users.UpdateOne(filter, update);
        }
    }
}
