using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IncidentManagementSystem.Models
{
    public class AssignedTo
    {
        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string userId { get; set; } = "";

        [BsonElement("firstName")]
        public string FirstName { get; set; } = "";

        [BsonElement("lastName")]
        public string LastName { get; set; } = "";
    }
}
