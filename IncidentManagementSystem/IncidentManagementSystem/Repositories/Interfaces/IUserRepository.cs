using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
        Task<User> GetUserByLoginAsync(string userName, string password, Role role);
        Task<User> EmailAddressExistsAsync(string emailAddress);
        Task AddTokenAsync(string email, string token, DateTime tokenExpiry);
        Task<User> CheckTokenValidityAsync(string token);
        Task ResetPasswordAsync(string token, string password);

        // User? GetById(string id);
        // void Update(User user);
        // void Delete(string id);
    }
}
