using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
<<<<<<< HEAD
        List<User> GetAllUsers();
        User? GetUserByLogin(string userName, string password, Role role);
        User EmailAddressExists(string emailAddress);
        User UserNameExists(string userName);
        void AddToken(string email, string token, DateTime tokenExpiry);
        User CheckTokenValidity(string token);
        void ResetPassword(string token, string password);
        List<string> GetDistinctBranches();

        User? GetById(string id);
        void AddUser(User user);


        void UpdateUser(string id, User updatedUser);
        void DeleteUser(string id);
=======
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
>>>>>>> 8269353a06638412e9570e569dd161708457849b
    }
}