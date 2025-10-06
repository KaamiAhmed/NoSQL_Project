using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAll();
        void Add(User user);
        User? GetUserByLogin(string userName, string password, Role role);
        User EmailAddressExists(string emailAddress);
        void AddToken(string email, string token, DateTime tokenExpiry);
        User CheckTokenValidity(string token);
        void ResetPassword(string token, string password);

        // User? GetById(string id);
        // void Update(User user);
        // void Delete(string id);
    }
}
