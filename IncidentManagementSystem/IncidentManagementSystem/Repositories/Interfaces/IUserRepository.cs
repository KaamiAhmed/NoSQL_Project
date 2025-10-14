using IncidentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IncidentManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        // -------- Auth / lookups --------
        User? GetUserByLogin(string userName, string password, Role role);

        // -------- Read --------
        User? GetById(string id);
        Task<User?> GetByIdAsync(string id);

        List<User> GetAllUsers();
        Task<List<User>> GetAllUsersAsync();

        List<string> GetDistinctBranches();
        Task<List<string>> GetDistinctBranchesAsync();

        // -------- Create --------
        void AddUser(User user);
        Task AddUserAsync(User user);

        // -------- Update --------
        void UpdateUser(string id, User updatedUser);
        Task UpdateUserAsync(string id, User updatedUser);

        // -------- Delete --------
        void DeleteUser(string id);
        Task DeleteUserAsync(string id);

        // -------- Existence checks --------
        Task<bool> EmailAddressExistsAsync(string emailAddress);
        User? UserNameExists(string userName);

        // -------- Password reset token flow (sync in repo; async wrappers in service) --------
        void AddToken(string email, string token, DateTime tokenExpiry);
        User? CheckTokenValidity(string token);
        void ResetPassword(string token, string password);
    }
}
