using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace UserService.Services
{
    public class ServerListService
    {
        private readonly IConfiguration config;
        public ServerListService(IConfiguration configuration)
        {
            config = configuration;
        }

        public async Task<BsonDocument> GetServerList(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id" }))
                return new BsonDocument();

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();      

            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var res = BsonDocument.Create(await users.Find(Builders<BsonDocument>.Filter.Eq("_Id", ObjectId.Parse(dict["_id"].ToString()))).Project(Builders<BsonDocument>.Projection.Include("Servers")).FirstOrDefaultAsync());

            db.Close();

            return res ?? new BsonDocument();
        }

        public async Task<bool> AddToServerList(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "server" }))
                return false;

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Push("Servers", ObjectId.Parse(dict["server"].ToString())).Set("DateModified", DateTime.Now));
            
            db.Close();

            return true;
        }

        public async Task<bool> RemoveFromServerList(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id" }))
                return false;

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Pull("Servers", ObjectId.Parse(dict["server"].ToString())).Set("DateModified", DateTime.Now));

            db.Close();

            return true;
        }
    }
}
