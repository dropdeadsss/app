using MongoDB.Bson;

namespace AuthService.Models
{
    public class Credentials
    {
        public ObjectId Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateModified { get; set; }
    }
}
