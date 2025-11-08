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
            try
            {
                return await _users.Find(u => u.UserName == userName && u.Password == password && u.Role == role).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching user.", ex);
            }
        }

        public async Task<User> GetByIdAsync(string id)
        {
            try
            {
                return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user by ID.", ex);
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _users.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all users.", ex);
            }
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                await _users.InsertOneAsync(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding user.", ex);
            }
        }

        public async Task UpdateUserAsync(string id, User updatedUser)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.Id, id);
                var update = Builders<User>.Update
                    .Set(u => u.UserName, updatedUser.UserName)
                    .Set(u => u.Email, updatedUser.Email)
                    .Set(u => u.Role, updatedUser.Role)
                    .Set(u => u.Name.FirstName, updatedUser.Name.FirstName)
                    .Set(u => u.Name.LastName, updatedUser.Name.LastName)
                    .Set(u => u.Branch, updatedUser.Branch);

                var result = await _users.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                    throw new Exception("User not found for update.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user.", ex);
            }
        }

        public async Task DeleteUserAsync(string id)
        {
            try
            {
                var result = await _users.DeleteOneAsync(u => u.Id == id);
                if (result.DeletedCount == 0)
                    throw new Exception("User not found for deletion.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting user.", ex);
            }
        }

        public async Task<User> UserNameExistsAsync(string userName)
        {
            try
            {
                return await _users.Find(u => u.UserName == userName).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking username existence.", ex);
            }
        }

        public async Task<User> EmailExistsAsync(string email)
        {
            try
            {
                return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking email existence.", ex);
            }
        }

        public async Task<List<string>> GetDistinctBranchesAsync()
        {
            try
            {
                return await _users.Distinct<string>("branch", FilterDefinition<User>.Empty).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving distinct branches.", ex);
            }
        }
    }
}
