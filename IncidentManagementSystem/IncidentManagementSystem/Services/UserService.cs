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
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService(IUserRepository userRepository, IHttpContextAccessor contextAccessor)
        {
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
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

            if (await _userRepository.EmailAddressExistsAsync(user.Email))
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
            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            {
                updatedUser.Password = HashPassword(updatedUser.Password);
            }
            else
            {
                var existing = await _userRepository.GetByIdAsync(id);
                if (existing != null)
                {
                    updatedUser.Password = existing.Password;
                }
            }

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

        public async Task<bool> EmailAddressExistsAsync(string emailAddress)
        {
            return await _userRepository.EmailAddressExistsAsync(emailAddress);
        }

        public async Task<User> UserNameExistsAsync(string userName)
        {
            return await _userRepository.UserNameExistsAsync(userName);
        }

        public async Task ForgotPasswordAsync(string email, string resetLink)
        {
            if (!await _userRepository.EmailAddressExistsAsync(email))
            {
                throw new Exception("Email address not found.");
            }

            // Generate token
            string token = Guid.NewGuid().ToString();
            DateTime tokenExpiry = DateTime.UtcNow.AddMinutes(15);

            //save token in the database
            await _userRepository.AddTokenAsync(email, token, tokenExpiry);

            string resetUrl = $"{resetLink}?token={WebUtility.UrlEncode(token)}";

            //send email
            SendEmail(email, resetUrl);
        }

        private void SendEmail(string email, string resetUrl)
        {
            string? host = Environment.GetEnvironmentVariable("SMTP_HOST");
            int port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT"));
            bool enableSsl = bool.Parse(Environment.GetEnvironmentVariable("SMTP_EnableSsl"));
            bool useDefaultCredentials = bool.Parse(Environment.GetEnvironmentVariable("SMTP_UseDefaultCredentials"));
            string? username = Environment.GetEnvironmentVariable("SMTP_EMAIL");
            string? password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            //build MailMessage
            MailAddress fromEmailAddress = new MailAddress(username);
            MailAddress toEmailAddress = new MailAddress(email);

            MailMessage message = new MailMessage(fromEmailAddress, toEmailAddress);
            message.Subject = "Reset Password";
            message.Body = $"Please click on the link to reset your password. <a href='{resetUrl}'>Reset Password</a><br> The link will expire after 15 minutes.";
            message.IsBodyHtml = true;

            //use smtp to send mail
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = host;
            smtpClient.Port = port;
            smtpClient.EnableSsl = enableSsl;
            smtpClient.UseDefaultCredentials = useDefaultCredentials;
            smtpClient.Credentials = new NetworkCredential(fromEmailAddress.Address, password);

            smtpClient.Send(message);
        }

        public async Task AddTokenAsync(string email, string token, DateTime tokenExpiry)
        {
            await _userRepository.AddTokenAsync(email, token, tokenExpiry);
        }

        public async Task<User> CheckTokenValidityAsync(string token)
        {
            return await _userRepository.CheckTokenValidityAsync(token);
        }

        public async Task ResetPasswordAsync(string token, string password)
        {
            await _userRepository.ResetPasswordAsync(token, HashPassword(password));
        }
        public async Task<List<User>> GetUsersByRoleAsync(Role role)
        {
            var allUsers = await _userRepository.GetAllUsersAsync();
            return allUsers.Where(u => u.Role == role).ToList();
        }


    }
}
