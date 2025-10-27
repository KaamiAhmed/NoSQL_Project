using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using IncidentManagementSystem.Services.Interfaces;
using MongoDB.Driver;
using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace IncidentManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IResetPasswordRepository _resetPasswordRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService(IUserRepository userRepository, IHttpContextAccessor contextAccessor, IResetPasswordRepository resetPasswordRepository)
        {
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            _resetPasswordRepository = resetPasswordRepository;
        }

        public User? GetLoggedInUser()
        {
            User? loggedInUser = null;
            string? userJson = _contextAccessor.HttpContext?.Session.GetString("LoggedInUser");

            if (userJson != null)
            {
                loggedInUser = JsonSerializer.Deserialize<User>(userJson);
            }

            return loggedInUser;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<List<string>> GetDistinctBranchesAsync()
        {
            return await _userRepository.GetDistinctBranchesAsync();
        }

        public async Task AddUserAsync(User user)
        {
            // Uniqueness checks
            if (await _userRepository.UserNameExistsAsync(user.UserName) != null)
                throw new Exception("Username already exists.");

            if (await _resetPasswordRepository.EmailAddressExistsAsync(user.Email))
                throw new Exception("Email address already exists.");
            user.Password = HashPassword(user.Password);
            await _userRepository.AddUserAsync(user);
        }
            
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task UpdateUserAsync(string id, User updatedUser)
        {
            await _userRepository.UpdateUserAsync(id, updatedUser);
        }
        public async Task DeleteUserAsync(string id)
        {
            await _userRepository.DeleteUserAsync(id);
        }

        public async Task<User> GetUserByLoginAsync(string userName, string password, Role role)
        {
            return await _userRepository.GetUserByLoginAsync(userName, HashPassword(password), role);
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }

        public async Task<User> UserNameExistsAsync(string userName)
        {
            return await _userRepository.UserNameExistsAsync(userName);
        }

       public async Task<List<User>> GetUsersByRoleAsync(Role role)
        {
            var allUsers = await _userRepository.GetAllUsersAsync();
            return allUsers.Where(u => u.Role == role).ToList();
        }

    }
}
