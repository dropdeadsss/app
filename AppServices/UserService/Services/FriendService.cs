using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace UserService.Services
{
    public class FriendService
    {
        private readonly IConfiguration config;
        public FriendService(IConfiguration configuration)
        {
            config = configuration;
        }
        
        public async Task<string> AddFriend(JsonElement json)
        {
            var dict =  CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() {"_id", "friend"} ))
                return "Ошибка операции.";

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Push("FriendRequests", ObjectId.Parse(dict["friend"].ToString())).Set("DateModified", DateTime.Now));
            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["friend"].ToString())), Builders<BsonDocument>.Update.Push("FriendInvites", ObjectId.Parse(dict["_id"].ToString())).Set("DateModified", DateTime.Now));

            db.Close();

            return "Заявка в друзья отправлена.";
        }

        public async Task<string> RemoveFriend(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "friend" }))
                return "Ошибка операции.";

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();
            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Pull("Friends", ObjectId.Parse(dict["friend"].ToString())).Set("DateModified", DateTime.Now));
            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["friend"].ToString())), Builders<BsonDocument>.Update.Pull("Friends", ObjectId.Parse(dict["_id"].ToString())).Set("DateModified", DateTime.Now));

            db.Close();

            return "Пользователь удален из списка друзей.";
        }

        public async Task<string> AcceptFriendRequest(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "friend" }))
                return "Ошибка операции.";

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();
            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Push("Friends", ObjectId.Parse(dict["friend"].ToString())).Set("DateModified", DateTime.Now));
            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Pull("FriendInvites", ObjectId.Parse(dict["friend"].ToString())).Set("DateModified", DateTime.Now));
            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["friend"].ToString())), Builders<BsonDocument>.Update.Push("Friends", ObjectId.Parse(dict["_id"].ToString())).Set("DateModified", DateTime.Now));
            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["friend"].ToString())), Builders<BsonDocument>.Update.Pull("FriendRequests", ObjectId.Parse(dict["_id"].ToString())).Set("DateModified", DateTime.Now));

            db.Close();

            return "Заявка в друзья принята.";
        }

        public async Task<string> DenyFriendRequest(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "friend" }))
                return "Ошибка операции.";

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();
            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Pull("FriendInvites", ObjectId.Parse(dict["friend"].ToString())).Set("DateModified", DateTime.Now));
            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["friend"].ToString())), Builders<BsonDocument>.Update.Pull("FriendRequests", ObjectId.Parse(dict["_id"].ToString())).Set("DateModified", DateTime.Now));

            db.Close();

            return "Заявка в друзья отклонена.";
        }

        public async Task<string> CheckFriend(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "friend" }))
                return "Ошибка операции.";

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();
            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var filter = Builders<BsonDocument>.Filter.And(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Filter.Or(Builders<BsonDocument>.Filter.AnyEq("Friends", ObjectId.Parse(dict["friend"].ToString())), Builders<BsonDocument>.Filter.AnyEq("FriendRequests", ObjectId.Parse(dict["friend"].ToString())), Builders<BsonDocument>.Filter.AnyEq("FriendInvites", ObjectId.Parse(dict["friend"].ToString()))));

            if (await users.Find(filter).AnyAsync())
                return "Заявка отправлена пользователю или он уже находится в вашем списке друзей.";

            db.Close();

            return string.Empty;
        }


        public async Task<BsonDocument> GetFriends(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id"}))
                return new BsonDocument();

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var projection = Builders<BsonDocument>.Projection.Include("Username").Include("Nickname").Include("_id").Include("Description").Include("ProfileImg").Include("CreatedAt");

            var friends = await users.Find(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString()))).Project(Builders<BsonDocument>.Projection.Include("Friends")).FirstOrDefaultAsync();

            var res = BsonDocument.Create(await users.Find(Builders<BsonDocument>.Filter.In("_id", friends)).Project(projection).ToListAsync());

            db.Close();

            return res ?? new BsonDocument();
        }

        public async Task<BsonDocument> GetReqeusts(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id" }))
                return new BsonDocument();

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var projection = Builders<BsonDocument>.Projection.Include("Username").Include("Nickname").Include("_id").Include("Description").Include("ProfileImg").Include("CreatedAt");

            var requests = await users.Find(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString()))).Project(Builders<BsonDocument>.Projection.Include("FriendRequests")).FirstOrDefaultAsync();

            var res = BsonDocument.Create(await users.Find(Builders<BsonDocument>.Filter.In("_id", requests)).Project(projection).ToListAsync());

            db.Close();

            return res ?? new BsonDocument();
        }

        public async Task<BsonDocument> GetInvites(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id" }))
                return new BsonDocument();

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var projection = Builders<BsonDocument>.Projection.Include("Username").Include("Nickname").Include("_id").Include("Description").Include("ProfileImg").Include("CreatedAt");

            var invites = await users.Find(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString()))).Project(Builders<BsonDocument>.Projection.Include("FriendInvites")).FirstOrDefaultAsync();

            var res = BsonDocument.Create(await users.Find(Builders<BsonDocument>.Filter.In("_id", invites)).Project(projection).ToListAsync());

            db.Close();

            return res ?? new BsonDocument();
        }
    }
}
