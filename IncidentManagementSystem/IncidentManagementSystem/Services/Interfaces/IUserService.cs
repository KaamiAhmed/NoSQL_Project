using IncidentManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IncidentManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        // Session / context
        User? GetLoggedInUser();

        // Read
        Task<User?> GetByIdAsync(string id);
        Task<List<User>> GetAllUsersAsync();
        List<string> GetDistinctBranches();

        // Create
        Task AddUserAsync(User user);

        // Update
        Task UpdateUserAsync(string id, User updatedUser);

        // Delete
        Task DeleteUserAsync(string id);

        // Auth
        User? GetUserByLogin(string userName, string password, Role role);

        // Existence checks
        Task<bool> EmailAddressExistsAsync(string emailAddress);
        User? UserNameExists(string userName);

        // Password reset flow
        Task ForgotPassword(string email, string resetLink);
        Task<User?> CheckTokenValidityAsync(string token);
        Task ResetPasswordAsync(string token, string password);
    }
}
