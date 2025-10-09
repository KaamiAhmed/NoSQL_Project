using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
        Task<User> GetUserByLoginAsync(string userName, string password, Role role);
        Task<User> EmailAddressExistsAsync(string emailAddress);
        Task ForgotPasswordAsync(string email, string resetLink);
        Task<User> CheckTokenValidityAsync(string token);
        Task ResetPasswordAsync(string token, string password);
    }
}
