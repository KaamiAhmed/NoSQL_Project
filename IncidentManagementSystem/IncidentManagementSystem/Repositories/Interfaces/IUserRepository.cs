using IncidentManagementSystem.Models;

namespace IncidentManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAll();
        void Add(User user);
        User? GetUserByLogin(string userName, string password, Role role);

        // User? GetById(string id);
        // void Update(User user);
        // void Delete(string id);
    }
}
