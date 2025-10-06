using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        List<User> GetAll();
        void Add(User user);
        User? GetUserByLogin(string userName, string password, Role role);
        User EmailAddressExists(string emailAddress);
        void ForgotPassword(string email, string resetLink);
        User CheckTokenValidity(string token);
        void ResetPassword(string token, string password);
    }
}
