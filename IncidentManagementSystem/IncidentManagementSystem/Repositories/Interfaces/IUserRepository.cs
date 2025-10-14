using IncidentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IncidentManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        
        Task<User> GetUserByLoginAsync(string userName, string password, Role role);
        Task<User> GetByIdAsync(string id);

        Task<List<User>> GetAllUsersAsync();
        Task<User> UserNameExistsAsync(string userName);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(string id, User updatedUser);
        Task DeleteUserAsync(string id);
        Task<List<string>> GetDistinctBranchesAsync();
        Task ForgotPasswordAsync(string email, string resetLink);
        Task<User> CheckTokenValidityAsync(string token);
        Task ResetPasswordAsync(string token, string password);

    }
}
