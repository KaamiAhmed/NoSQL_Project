using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IncidentManagementSystem.Models
{
    public class Ticket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = "";

        [BsonElement("description")]
        public string Description { get; set; } = "";

        [BsonElement("typeofincident")]
        [BsonRepresentation(BsonType.String)]
        public TypeOfIncident TypeOfIncident { get; set; }

        [BsonElement("dateofevent")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime DateOfEvent { get; set; }

        [BsonElement("reportedby")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ReportedBy { get; set; }

        [BsonIgnore]
        public string ReportedByName { get; set; } = "";

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public Status Status { get; set; }

        [BsonElement("deadline")]
        public int Deadline { get; set; }

        [BsonElement("priority")]
        [BsonRepresentation(BsonType.String)]
        public Priority Priority { get; set; }

        [BsonElement("assignedto")]
        public AssignedTo? AssignedTo { get; set; }
    }
}
