using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using MongoDB.Driver;

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

        public User? GetById(string id)
        {
            return _users.Find(u => u.Id == id).FirstOrDefault();
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        // ---------- Read ----------
        public List<User> GetAllUsers()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        // ---------- Create ----------
        public void AddUser(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task AddUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        // ---------- Update ----------
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

        // ---------- Delete ----------
        public void DeleteUser(string id)
        {
            _users.DeleteOne(u => u.Id == id);
        }

        public async Task DeleteUserAsync(string id)
        {
            await _users.DeleteOneAsync(u => u.Id == id);
        }

        // ---------- Existence checks ----------
        public async Task<bool> EmailAddressExistsAsync(string emailAddress)
        {
            return await _users.Find(u => u.Email == emailAddress).AnyAsync();
        }

        public async Task<User> UserNameExistsAsync(string userName)
        {
            return await _users.Find(u => u.UserName == userName).FirstOrDefaultAsync();
        }

        public async Task AddTokenAsync(string email, string token, DateTime tokenExpiry)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var update = Builders<User>.Update.Set(u => u.ResetPasswordViewModel.Token, token).Set(u => u.ResetPasswordViewModel.TokenExpiry, tokenExpiry);
            await _users.UpdateOneAsync(filter, update);
        }

        public async Task<User> CheckTokenValidityAsync(string token)
        {
            return await _users.Find(user => user.ResetPasswordViewModel.Token == token && user.ResetPasswordViewModel.TokenExpiry > DateTime.UtcNow).FirstOrDefaultAsync();
        }

        public async Task ResetPasswordAsync(string token, string password)
        {
            var filter = Builders<User>.Filter.Eq(u => u.ResetPasswordViewModel.Token, token);
            var update = Builders<User>.Update.Set(u => u.Password, password).Unset(u => u.ResetPasswordViewModel);
            await _users.UpdateOneAsync(filter, update);
        }

        public async Task<List<string>> GetDistinctBranchesAsync()
        {
            return await _users.Distinct<string>("branch", FilterDefinition<User>.Empty).ToListAsync();
        }

        public async Task<List<string>> GetDistinctBranchesAsync()
        {
            return await _users.Distinct<string>("branch", FilterDefinition<User>.Empty).ToListAsync();
        }
    }
}
