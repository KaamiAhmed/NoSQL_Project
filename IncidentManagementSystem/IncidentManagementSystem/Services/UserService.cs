using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories;
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
        private readonly ITicketService _ticketService;

        public UserService(IUserRepository userRepository, IHttpContextAccessor contextAccessor, IResetPasswordRepository resetPasswordRepository, ITicketService ticketService)
        {
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            _resetPasswordRepository = resetPasswordRepository;
            _ticketService = ticketService;

        }

        public User? GetLoggedInUser()
        {
            try
            {
                User? loggedInUser = null;
                string? userJson = _contextAccessor.HttpContext?.Session.GetString("LoggedInUser");

                if (userJson != null)
                {
                    loggedInUser = JsonSerializer.Deserialize<User>(userJson);
                }

                return loggedInUser;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving logged-in user from session.", ex);
            }
        }

        public async Task<User> GetByIdAsync(string id)
        {
            try
            {
                return await _userRepository.GetByIdAsync(id);
            }
            catch (MongoException ex)
            {
                throw new Exception("Database error while getting user by ID.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting user by ID.", ex);
            }
        }

        public async Task<List<string>> GetDistinctBranchesAsync()
        {
            try
            {
                return await _userRepository.GetDistinctBranchesAsync();
            }
            catch (MongoException ex)
            {
                throw new Exception("Database error while getting branches.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting branches.", ex);
            }
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                if (await _userRepository.UserNameExistsAsync(user.UserName) != null)
                    throw new Exception("Username already exists.");

                if (await _userRepository.EmailExistsAsync(user.Email) != null)
                    throw new Exception("Email address already exists.");

                user.Password = HashPassword(user.Password);

                await _userRepository.AddUserAsync(user);
            }
            catch (MongoException ex)
            {
                throw new Exception("Error in the database while creating the user.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _userRepository.GetAllUsersAsync();
            }
            catch (MongoException ex)
            {
                throw new Exception("Database error while retrieving all users.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving all users.", ex);
            }
        }

        public async Task UpdateUserAsync(string id, User updatedUser)
        {
            try
            {
                var allUsers = await _userRepository.GetAllUsersAsync();

                if (allUsers.Any(u => u.UserName == updatedUser.UserName && u.Id != id))
                    throw new Exception("Username already exists.");
                if (allUsers.Any(u => u.Email == updatedUser.Email && u.Id != id))
                    throw new Exception("Email address already exists.");
                await _userRepository.UpdateUserAsync(id, updatedUser);
            }
            catch (MongoException ex)
            {
                throw new Exception("Error in the database while updating the user.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task DeleteUserAsync(string id)
        {
            try
            {
                await _ticketService.UnassignTicketsFromUserAsync(id);
                await _userRepository.DeleteUserAsync(id);
            }
            catch (MongoException ex)
            {
                throw new Exception("Error in the database while deleting the user.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while deleting the user.", ex);
            }
        }

        public async Task<User> GetUserByLoginAsync(string userName, string password, Role role)
        {
            try
            {
                return await _userRepository.GetUserByLoginAsync(userName, HashPassword(password), role);
            }
            catch (MongoException ex)
            {
                throw new Exception("Database error during login.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login.", ex);
            }
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
            try
            {
                return await _userRepository.UserNameExistsAsync(userName);
            }
            catch (MongoException ex)
            {
                throw new Exception("Database error while checking username.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while checking username.", ex);
            }
        }

        public async Task<List<User>> GetUsersByRoleAsync(Role role)
        {
            try
            {
                var allUsers = await _userRepository.GetAllUsersAsync();
                return allUsers.Where(u => u.Role == role).ToList();
            }
            catch (MongoException ex)
            {
                throw new Exception("Database error while getting users by role.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting users by role.", ex);
            }
        }
    }
}
