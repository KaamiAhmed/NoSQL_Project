using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using IncidentManagementSystem.Services.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace IncidentManagementSystem.Services
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly IResetPasswordRepository _resetPasswordRepository;

        public ResetPasswordService(IResetPasswordRepository resetPasswordRepository)
        {
            _resetPasswordRepository = resetPasswordRepository;
        }

        public async Task ForgotPasswordAsync(string email, string resetLink)
        {
            if (!await _resetPasswordRepository.EmailAddressExistsAsync(email))
            {
                throw new Exception("Email address not found.");
            }

            // Generate token
            string token = Guid.NewGuid().ToString();
            DateTime tokenExpiry = DateTime.UtcNow.AddMinutes(15);

            //save token in the database
            await _resetPasswordRepository.AddTokenAsync(email, token, tokenExpiry);

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

        public async Task<User> CheckTokenValidityAsync(string token)
        {
            return await _resetPasswordRepository.CheckTokenValidityAsync(token);
        }

        public async Task ResetPasswordAsync(string token, string password)
        {
            string hashedPassword = HashPassword(password);
            if(await _resetPasswordRepository.GetOldPassword(token) == hashedPassword)
            {
                throw new Exception("You can not have the same password as previous one. Please set a new password.");
            }
            await _resetPasswordRepository.ResetPasswordAsync(token, hashedPassword);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
