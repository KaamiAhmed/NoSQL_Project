using IncidentManagementSystem.Models;

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
        Task<User> UserNameExistsAsync(string userName);
        Task ForgotPasswordAsync(string email, string resetLink);
        Task AddTokenAsync(string email, string token, DateTime tokenExpiry);
        Task<User> CheckTokenValidityAsync(string token);
        Task ResetPasswordAsync(string token, string password);
        
        Task UpdateUserAsync(string id, User updatedUser);

        
        Task DeleteUserAsync(string id);

        Task<List<User>> GetUsersByRoleAsync(Role role);



    }

}
