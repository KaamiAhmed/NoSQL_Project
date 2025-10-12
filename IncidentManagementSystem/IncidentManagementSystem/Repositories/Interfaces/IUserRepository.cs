using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAllUsers();
        User? GetUserByLogin(string userName, string password, Role role);
        User EmailAddressExists(string emailAddress);
        void AddToken(string email, string token, DateTime tokenExpiry);
        User CheckTokenValidity(string token);
        void ResetPassword(string token, string password);

        User? GetById(string id);
        void AddUser(User user);

        void UpdateUser(string id, User updatedUser);
        void DeleteUser(string id);
    }
}