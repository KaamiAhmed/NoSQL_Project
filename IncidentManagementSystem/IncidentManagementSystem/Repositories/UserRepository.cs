using IncidentManagementSystem.Models;
using IncidentManagementSystem.Repositories.Interfaces;
using MongoDB.Driver;
namespace IncidentManagementSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase db)
        {
            _users = db.GetCollection<User>("User");
        }

        public User GetUserByLogin(string userName, string password, Role role)
        {
            return _users.Find(u => u.UserName == userName && u.Password == password && u.Role == role).FirstOrDefault();
        }

        public List<User> GetAll()
        {
            return _users.Find(_ => true).ToList();
        }

        public void Add(User user) =>
            _users.InsertOne(user);

    }
}
