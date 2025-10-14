using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using IncidentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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

        // ---------------- Session / Context ----------------
        public User? GetLoggedInUser()
        {
            string? userJson = _contextAccessor.HttpContext?.Session.GetString("LoggedInUser");
            return userJson != null ? JsonSerializer.Deserialize<User>(userJson) : null;
        }

        // ---------------- Read ----------------
        public async Task<User?> GetByIdAsync(string id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public List<string> GetDistinctBranches()
        {
            return _userRepository.GetDistinctBranches();
        }

        // ---------------- Create ----------------
        public async Task AddUserAsync(User user)
        {
            // Uniqueness checks
            if (_userRepository.UserNameExists(user.UserName) != null)
                throw new Exception("Username already exists.");

            if (await _userRepository.EmailAddressExistsAsync(user.Email))
                throw new Exception("Email address already exists.");

            user.Password = HashPassword(user.Password);
            await _userRepository.AddUserAsync(user);
        }

        // ---------------- Update ----------------
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
                    // preserve current hashed password
                    updatedUser.Password = existing.Password;
                }
            }

            await _userRepository.UpdateUserAsync(id, updatedUser);
        }

        // ---------------- Delete ----------------
        public async Task DeleteUserAsync(string id)
        {
            await _userRepository.DeleteUserAsync(id);
        }

        // ---------------- Auth ----------------
        public User? GetUserByLogin(string userName, string password, Role role)
        {
            return _userRepository.GetUserByLogin(userName, HashPassword(password), role);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }

        // ---------------- Existence checks ----------------
        public async Task<bool> EmailAddressExistsAsync(string emailAddress)
        {
            return await _userRepository.EmailAddressExistsAsync(emailAddress);
        }

        public User? UserNameExists(string userName)
        {
            return _userRepository.UserNameExists(userName);
        }

        // ---------------- Password reset flow ----------------
        public async Task ForgotPassword(string email, string resetLink)
        {
            if (!await _userRepository.EmailAddressExistsAsync(email))
                throw new Exception("Email address not found.");

            // Generate token
            string token = Guid.NewGuid().ToString();
            DateTime tokenExpiry = DateTime.UtcNow.AddMinutes(15);

            // Save token (repository has sync method; that’s fine here)
            _userRepository.AddToken(email, token, tokenExpiry);

            string resetUrl = $"{resetLink}?token={WebUtility.UrlEncode(token)}";

            // Send email
            SendEmail(email, resetUrl);
        }

        private void SendEmail(string email, string resetUrl)
        {
            // Read SMTP config (with basic fallbacks to avoid null refs)
            string host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "localhost";
            int port = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var p) ? p : 25;
            bool enableSsl = bool.TryParse(Environment.GetEnvironmentVariable("SMTP_EnableSsl"), out var ssl) && ssl;
            bool useDefaultCredentials = bool.TryParse(Environment.GetEnvironmentVariable("SMTP_UseDefaultCredentials"), out var udc) && udc;
            string username = Environment.GetEnvironmentVariable("SMTP_EMAIL") ?? "no-reply@example.com";
            string password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? string.Empty;

            var fromEmailAddress = new MailAddress(username);
            var toEmailAddress = new MailAddress(email);

            var message = new MailMessage(fromEmailAddress, toEmailAddress)
            {
                Subject = "Reset Password",
                Body = $"Please click the link to reset your password: <a href='{resetUrl}'>Reset Password</a><br/>The link will expire after 15 minutes.",
                IsBodyHtml = true
            };

            using var smtpClient = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = enableSsl,
                UseDefaultCredentials = useDefaultCredentials,
                Credentials = useDefaultCredentials ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(fromEmailAddress.Address, password)
            };

            smtpClient.Send(message);
        }

        // Async-style wrappers over sync repository methods (keeps your async controller/service flow)
        public Task<User?> CheckTokenValidityAsync(string token)
        {
            var user = _userRepository.CheckTokenValidity(token);
            return Task.FromResult(user);
        }

        public Task ResetPasswordAsync(string token, string password)
        {
            _userRepository.ResetPassword(token, HashPassword(password));
            return Task.CompletedTask;
        }
    }
}
