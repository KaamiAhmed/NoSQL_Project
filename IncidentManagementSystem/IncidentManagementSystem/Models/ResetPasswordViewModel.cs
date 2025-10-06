using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.Contracts;

namespace IncidentManagementSystem.Models
{
    public class ResetPasswordViewModel
    {
        [BsonElement("token")]
        [BsonIgnoreIfNull]
        public string Token { get; set; } = "";

        [BsonElement("tokenExpiry")]
        [BsonIgnoreIfNull]
        public DateTime TokenExpiry { get; set; }

        [BsonElement("password")]
        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmPassword {  get; set; } = string.Empty;
    }
}
