using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Services.Interfaces
{
    public interface IResetPasswordService
    {
        Task ForgotPasswordAsync(string email, string resetLink);

        Task<User> CheckTokenValidityAsync(string token);
        Task ResetPasswordAsync(string token, string password);
    }
}
