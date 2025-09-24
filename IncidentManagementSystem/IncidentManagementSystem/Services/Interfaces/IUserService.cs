using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        List<User> GetAll();
        void Add(User user);
        User? GetUserByLogin(string userName, string password, string role);
    }
}
