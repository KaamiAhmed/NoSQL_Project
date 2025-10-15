using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IncidentManagementSystem.Models
{
    public class AssignedTo 
    {
        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string userId { get; set; } = "";

        [BsonElement("firstname")]
        public string FirstName { get; set; } = "";

        [BsonElement("lastname")]
        public string LastName { get; set; } = "";
    }
}
