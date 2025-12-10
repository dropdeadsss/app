using MongoDB.Bson;
using System.Text.Json;

namespace UserService.Models
{
    public class User
    {    
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public byte[] ProfileImg { get; set; }
        public string Description { get; set; }
        public List<ObjectId> Friends { get; set; }
        public List<ObjectId> BlockedUsers {  get; set; }
        public List<ObjectId> ReceivedFriendInvites { get; set; }
        public List<ObjectId> SentFriendInvites { get; set; }
        public List<ObjectId> Servers { get; set; }
        public Settings Settings { get; set; }
    }
}
