using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        List<User> GetAllUsers();
        User? GetById(string id);
        void AddUser(User user);
        void UpdateUser(string id, User updatedUser);
        void DeleteUser(string id);
        User? GetUserByLogin(string userName, string password, Role role);
        User EmailAddressExists(string emailAddress);
        void ForgotPassword(string email, string resetLink);
        User CheckTokenValidity(string token);
        void ResetPassword(string token, string password);
    }
}
