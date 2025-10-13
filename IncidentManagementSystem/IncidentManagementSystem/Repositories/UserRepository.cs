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

        public async Task<User> GetByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return _users.Find(_ => true).ToList();
        }

        public async Task AddUserAsync(User user)
        {
            _users.InsertOne(user);
        }

        public async Task UpdateUserAsync(string id, User updatedUser)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var update = Builders<User>.Update
                .Set(u => u.UserName, updatedUser.UserName)
                .Set(u => u.Email, updatedUser.Email)
                .Set(u => u.Password, updatedUser.Password)
                .Set(u => u.Role, updatedUser.Role);

            await _users.UpdateOneAsync(filter, update);
        }

        public async Task DeleteUserAsync(string id)
        {
            await _users.DeleteOneAsync(u => u.Id == id);
        }

        public async Task<bool> EmailAddressExistsAsync(string emailAddress)
        {
            //long count = await _users.CountDocumentsAsync(Builders<User>.Filter.Eq(u => u.Email, emailAddress));
            return await _users.Find(u => u.Email == emailAddress).AnyAsync();
        }

        public void AddToken(string email, string token, DateTime tokenExpiry)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var update = Builders<User>.Update.Set(u => u.ResetPasswordViewModel.Token, token).Set(u => u.ResetPasswordViewModel.TokenExpiry, tokenExpiry);
            _users.UpdateOne(filter, update);
        }

        public User CheckTokenValidity(string token)
        {
            return _users.Find(user => user.ResetPasswordViewModel.Token == token && user.ResetPasswordViewModel.TokenExpiry > DateTime.UtcNow).FirstOrDefault();
        }

        public void ResetPassword(string token, string password)
        {
            var filter = Builders<User>.Filter.Eq(u => u.ResetPasswordViewModel.Token, token);
            var update = Builders<User>.Update.Set(u => u.Password, password).Unset(u => u.ResetPasswordViewModel);
            _users.UpdateOne(filter, update);
        }
    }
}
