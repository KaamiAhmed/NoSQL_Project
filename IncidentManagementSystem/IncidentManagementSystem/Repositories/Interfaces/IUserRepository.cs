using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByLoginAsync(string userName, string password, Role role);
        Task<User> GetByIdAsync(string id);

        Task<List<User>> GetAllUsersAsync();
        Task<bool> EmailAddressExistsAsync(string emailAddress);
        Task<User> UserNameExistsAsync(string userName);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(string id, User updatedUser);
        Task DeleteUserAsync(string id);
        Task<List<string>> GetDistinctBranchesAsync();
        Task AddTokenAsync(string email, string token, DateTime tokenExpiry);
        Task<User> CheckTokenValidityAsync(string token);
        Task ResetPasswordAsync(string token, string password);
    }
}