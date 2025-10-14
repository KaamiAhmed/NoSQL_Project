using IncidentManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IncidentManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
       
        User? GetLoggedInUser();

       
        Task<User> GetByIdAsync(string id);
        Task<List<User>> GetAllUsersAsync();
        Task<List<string>> GetDistinctBranchesAsync();
        Task AddUserAsync(User user);
        Task<User> GetUserByLoginAsync(string userName, string password, Role role);
        Task<bool> EmailAddressExistsAsync(string emailAddress);
        Task<User> UserNameExists(string userName);
        Task ForgotPasswordAsync(string email, string resetLink);
        Task<User> CheckTokenValidityAsync(string token);
        Task ResetPasswordAsync(string token, string password);
        
        Task UpdateUserAsync(string id, User updatedUser);

        
        Task DeleteUserAsync(string id);

       

        
    }
}
