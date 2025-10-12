using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
        Task<User> GetUserByLoginAsync(string userName, string password, Role role);
        Task<bool> EmailAddressExistsAsync(string emailAddress);
        Task AddTokenAsync(string email, string token, DateTime tokenExpiry);
        Task<User> CheckTokenValidityAsync(string token);
        Task ResetPasswordAsync(string token, string password);
        Task<User> GetByIdAsync(string id);
        Task UpdateUserAsync(string id, User updatedUser);
        Task DeleteUserAsync(string id);
    }
}
