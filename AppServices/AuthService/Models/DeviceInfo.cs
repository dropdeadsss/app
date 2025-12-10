using MongoDB.Bson;
using System.Net;

namespace AuthService.Models
{
    public class DeviceInfo
    {
        public ObjectId Id { get; set; }
        public List<string> Ip { get; set; }
        public DateTime DateModified { get; set; }
    }
}
