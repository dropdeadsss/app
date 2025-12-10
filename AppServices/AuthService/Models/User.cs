using MongoDB.Bson;

namespace AuthService.Models
{
    public class User
    {
        public ObjectId? Id { get; set; }
        public string Username { get; set; }
        public DateOnly? Dob { get; set; }
        public string Phone { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }
    } 
}
