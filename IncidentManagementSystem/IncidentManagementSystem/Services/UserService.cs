using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using IncidentManagementSystem.Services.Interfaces;
using MongoDB.Driver;
using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace IncidentManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public User GetById(string id)
        {
            return _userRepository.GetById(id);
        }

        public void AddUser(User user)
        {
            user.Password = HashPassword(user.Password);
            _userRepository.AddUser(user);
        }

        public void UpdateUser(string id, User updatedUser)
        {
            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            {
                updatedUser.Password = HashPassword(updatedUser.Password);
            }
            else
            {
                var existing = _userRepository.GetById(id);
                if (existing != null)
                {
                    updatedUser.Password = existing.Password;
                }
            }
            _userRepository.UpdateUser(id, updatedUser);
        }
        public void DeleteUser(string id)
        {
            _userRepository.DeleteUser(id);
        }

        public User? GetUserByLogin(string userName, string password, Role role)
        {
            return _userRepository.GetUserByLogin(userName, HashPassword(password), role);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }

        public User EmailAddressExists(string emailAddress)
        {
            return _userRepository.EmailAddressExists(emailAddress);
        }

        public void ForgotPassword(string email, string resetLink)
        {
            User user = _userRepository.EmailAddressExists(email);
            if (user == null)
            {
                throw new Exception("Email address not found.");
            }

            // Generate token
            string token = Guid.NewGuid().ToString();
            DateTime tokenExpiry = DateTime.UtcNow.AddMinutes(15);

            //save token in the database
            _userRepository.AddToken(email, token, tokenExpiry);

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

        public User CheckTokenValidity(string token)
        {
            return _userRepository.CheckTokenValidity(token);
        }

        public void ResetPassword(string token, string password)
        {
            _userRepository.ResetPassword(token, HashPassword(password));
        }
    }
}
