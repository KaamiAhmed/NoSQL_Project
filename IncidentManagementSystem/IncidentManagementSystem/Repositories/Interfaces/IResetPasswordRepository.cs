using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Repositories.Interfaces
{
    public interface IResetPasswordRepository
    {
        Task<bool> EmailAddressExistsAsync(string emailAddress);
        Task AddTokenAsync(string email, string token, DateTime tokenExpiry);
        Task<User> CheckTokenValidityAsync(string token);
        Task ResetPasswordAsync(string token, string password);
        Task<string> GetOldPassword(string token);
    }
}
