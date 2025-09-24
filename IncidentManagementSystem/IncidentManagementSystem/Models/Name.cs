using MongoDB.Bson.Serialization.Attributes;

namespace IncidentManagementSystem.Models
{
    public class Name
    {
        [BsonElement("firstname")]
        public string FirstName { get; set; } = "";

        [BsonElement("lastname")]
        public string LastName { get; set; } = "";
    }
}
