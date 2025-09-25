using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IncidentManagementSystem.Models
{
    public class User
    {
        // This will be the primary key in MongoDB.
        // [BsonId] tells MongoDB this field is the "_id".
        // [BsonRepresentation(BsonType.ObjectId)] lets us use string in C#,
        // while MongoDB still stores it as a real ObjectId internally.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("username")]
        public string UserName { get; set; } = "";

        [BsonElement("password")]
        public string Password { get; set; } = "";

        // A simple string field for the user's name.
        // Default = "" so it's never null when creating a new User.
        [BsonElement("name")]
        public Name? Name { get; set; }

        // A simple string field for the user's email.
        // Later we could add validation (e.g. DataAnnotations).
        [BsonElement("email")]
        public string Email { get; set; } = "";

        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)]
        public Role Role { get; set; }

        [BsonElement("branch")]
        public string Branch { get; set; } = "";

    }
}
