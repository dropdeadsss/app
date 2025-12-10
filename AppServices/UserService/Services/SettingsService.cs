using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace UserService.Services
{
    public class SettingsService
    {
        private readonly IConfiguration config;
        public SettingsService(IConfiguration configuration)
        {
            config = configuration;
        }

        public async Task<bool> SaveSettings(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "audiooutput", "audioinput", "apptheme" }))
                return false;

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var settings = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_2"]);

            var doc = json.ToBsonDocument().Add(new BsonElement("DateModified", DateTime.Now));

            if (await settings.Find(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString()))).AnyAsync())
                await settings.FindOneAndReplaceAsync(Builders<BsonDocument>.Filter.Eq("_id", dict["_id"].ToString()), doc);
            else
                await settings.InsertOneAsync(doc);

            db.Close();

            return true;
        }

        public async Task<BsonDocument> GetSettings(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id" }))
                return new BsonDocument();

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var settings = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_2"]);

            var res = await settings.Find(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString()))).FirstOrDefaultAsync();

            db.Close();

            return res ?? new BsonDocument();
        }
    }
}
