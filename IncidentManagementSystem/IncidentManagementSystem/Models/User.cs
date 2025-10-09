using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IncidentManagementSystem.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("username")]
        public string UserName { get; set; } = "";

        [BsonElement("password")]
        public string Password { get; set; } = "";

        [BsonElement("name")]
        public Name? Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = "";

        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)]
        public Role Role { get; set; }

        [BsonElement("branch")]
        public string Branch { get; set; } = "";

        //for password reset
        public ResetPasswordViewModel? ResetPasswordViewModel { get; set; }

    }
}
