using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
        Task<User> GetUserByLoginAsync(string userName, string password, Role role);
        Task<bool> EmailAddressExistsAsync(string emailAddress);
        Task ForgotPasswordAsync(string email, string resetLink);
        Task<User> CheckTokenValidityAsync(string token);
        Task ResetPasswordAsync(string token, string password);
        Task<User> GetByIdAsync(string id);
        Task UpdateUserAsync(string id, User updatedUser);
        Task DeleteUserAsync(string id);
        User? GetLoggedInUser();
    }
}
