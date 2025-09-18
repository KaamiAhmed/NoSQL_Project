using MongoDB.Bson.Serialization.Attributes;

namespace IncidentManagementSystem.Models
{
    public class Login
    {
        [BsonElement("username")]
        public string UserName { get; set; } = "";

        [BsonElement("password")]
        public string Password { get; set; } = "";

        [BsonElement("role")]
        public string Role { get; set; } = "";
    }
}
