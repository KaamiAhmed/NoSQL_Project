using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using MongoDB.Driver;

namespace IncidentManagementSystem.Repositories
{
    public class ResetPasswordRepository : IResetPasswordRepository
    {
        private readonly IMongoCollection<User> _users;

        public ResetPasswordRepository(IMongoDatabase db)
        {
            _users = db.GetCollection<User>("User");
        }

        public async Task<bool> EmailAddressExistsAsync(string emailAddress)
        {
            //long count = await _users.CountDocumentsAsync(Builders<User>.Filter.Eq(u => u.Email, emailAddress));
            try
            {
                return await _users.Find(u => u.Email == emailAddress).AnyAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddTokenAsync(string email, string token, DateTime tokenExpiry)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var update = Builders<User>.Update.Set(u => u.ResetPasswordViewModel.Token, token).Set(u => u.ResetPasswordViewModel.TokenExpiry, tokenExpiry);
            try
            {
                await _users.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> CheckTokenValidityAsync(string token)
        {
            try
            {
                return await _users.Find(user => user.ResetPasswordViewModel.Token == token && user.ResetPasswordViewModel.TokenExpiry > DateTime.UtcNow).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ResetPasswordAsync(string token, string password)
        {
            var filter = Builders<User>.Filter.Eq(u => u.ResetPasswordViewModel.Token, token);
            var update = Builders<User>.Update.Set(u => u.Password, password).Unset(u => u.ResetPasswordViewModel);
            try
            {
                await _users.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetOldPassword(string token)
        {
            //using aggregation pipeline
            return await _users.Aggregate().Match(u => u.ResetPasswordViewModel.Token == token).Project(u => u.Password).FirstOrDefaultAsync();
        }
    }
}
