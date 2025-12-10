using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace UserService.Services
{
    public class BlockService
    {
        private readonly IConfiguration config;
        public BlockService(IConfiguration configuration)
        {
            config = configuration;
        }

        public async Task<string> BlockUser(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "user" }))
                return "Ошибка операции.";

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Push("Blocked", ObjectId.Parse(dict["user"].ToString())).Set("DateModified", DateTime.Now));

            db.Close();

            return "Пользователь заблокирован.";
        }

        public async Task<string> UnblockUser(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "user" }))
                return "Ошибка операции.";

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Pull("Blocked", ObjectId.Parse(dict["user"].ToString())).Set("DateModified", DateTime.Now));

            db.Close();

            return "Пользователь разблокирован.";
        }

        public async Task<string> CheckBlock(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "user" }))
                return "Ошибка операции.";

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var filter = Builders<BsonDocument>.Filter.And(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Filter.AnyEq("Blocked", ObjectId.Parse(dict["user"].ToString())));

            if(await users.Find(filter).AnyAsync())
                return "Пользователь уже заблокирован.";

            db.Close();

            return string.Empty;
        }

        public async Task<BsonDocument> GetBlocked(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id" }))
                return new BsonDocument();

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var projection = Builders<BsonDocument>.Projection.Include("Username").Include("Nickname").Include("_id").Include("Description").Include("ProfileImg").Include("CreatedAt");

            var blocked = await users.Find(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString()))).Project(Builders<BsonDocument>.Projection.Include("Blocked")).FirstOrDefaultAsync();

            var res = BsonDocument.Create(await users.Find(Builders<BsonDocument>.Filter.In("_id", blocked)).Project(projection).ToListAsync());

            db.Close();

            return res ?? new BsonDocument();
        }
        
    }
}