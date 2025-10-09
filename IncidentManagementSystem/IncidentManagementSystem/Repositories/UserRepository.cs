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

        public async Task<User> GetUserByLoginAsync(string userName, string password, Role role)
        {
            return await _users.Find(u => u.UserName == userName && u.Password == password && u.Role == role).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task<User> EmailAddressExistsAsync(string emailAddress)
        {
            return await _users.Find(u => u.Email == emailAddress).FirstOrDefaultAsync();
        }

        public async Task AddTokenAsync(string email, string token, DateTime tokenExpiry)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var update = Builders<User>.Update.Set(u => u.ResetPasswordViewModel.Token, token).Set(u => u.ResetPasswordViewModel.TokenExpiry, tokenExpiry);
            await _users.UpdateOneAsync(filter, update);
        }

        public async Task<User> CheckTokenValidityAsync(string token)
        {
            return await _users.Find(user => user.ResetPasswordViewModel.Token == token &&  user.ResetPasswordViewModel.TokenExpiry > DateTime.UtcNow).FirstOrDefaultAsync();
        }

        public async Task ResetPasswordAsync(string token, string password)
        {
            var filter = Builders<User>.Filter.Eq(u => u.ResetPasswordViewModel.Token, token);
            var update = Builders<User>.Update.Set(u => u.Password, password).Unset(u => u.ResetPasswordViewModel);
            await _users.UpdateOneAsync(filter, update);
        }
    }
}
