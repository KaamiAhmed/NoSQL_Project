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

        public void Add(User user)
        {
            _userRepository.Add(user);
        }

        public List<User> GetAll()
        {
            return _userRepository.GetAll();
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
            //build MailMessage
            MailAddress fromEmailAddress = new MailAddress("kamranahmedi1272@gmail.com");
            MailAddress toEmailAddress = new MailAddress(email);

            MailMessage message = new MailMessage(fromEmailAddress, toEmailAddress);
            message.Subject = "Reset Password";
            message.Body = $"Please click on the link to reset your password. <a href='{resetUrl}'>Reset Password</a><br> The link will expire after 15 minutes.";
            message.IsBodyHtml = true;

            //use smtp to send mail
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(fromEmailAddress.Address, "dfip bmmn fjrc elkp");

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
