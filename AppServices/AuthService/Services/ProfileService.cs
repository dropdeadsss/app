using AuthService.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using UserService.Services;

namespace AuthService.Services
{
    public class ProfileService
    {
        private readonly IConfiguration configuration;
        public ProfileService(IConfiguration conf) 
        {
            configuration = conf;
        }

        public async Task<bool> ChangeUsername(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "username" }))
                return false;

            MongoDB db = new MongoDB(configuration);

            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(configuration["Collections:Col_2"]);

            var res = await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", dict["_id"].ToString()), Builders<BsonDocument>.Update.Set("Username", dict["username"].ToString()).Set("DateModified", DateTime.Now));

            db.Close();

            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<bool> ChangeEmail( JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "email" }) || !DataValidators.ValidateEmail(dict["email"].ToString()))
                return false;

            MongoDB db = new MongoDB(configuration);

            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(configuration["Collections:Col_1"]);

            var res = await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", dict["_id"].ToString()), Builders<BsonDocument>.Update.Set("Email", dict["email"].ToString()).Set("DateModified", DateTime.Now));

            db.Close();

            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<bool> ChangePhone(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "phone" }))
                return false;

            MongoDB db = new MongoDB(configuration);

            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(configuration["Collections:Col_2"]);

            var res = await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", dict["_id"].ToString()), Builders<BsonDocument>.Update.Set("Phone", dict["phone"].ToString()).Set("DateModified", DateTime.Now));

            db.Close();

            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<BsonDocument> GetProfileInfo (JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id" }))
                return new BsonDocument();

            MongoDB db = new MongoDB(configuration);

            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(configuration["Collections:Col_2"]);
            var creds = dbcontext.GetCollection<BsonDocument>(configuration["Collections:Col_1"]);

            var userproject = Builders<BsonDocument>.Projection.Include("Phone");
            var credproject = Builders<BsonDocument>.Projection.Include("Email");

            var user = await users.Find(Builders<BsonDocument>.Filter.Eq("_id", dict["_id"].ToString())).FirstOrDefaultAsync();
            var cred = await creds.Find(Builders<BsonDocument>.Filter.Eq("_id", dict["_id"].ToString())).FirstOrDefaultAsync();

            if (user == null || cred == null)
                return new BsonDocument();

            var res = new BsonDocument();
            res.Add("Phone", user["Phone"].ToString().Substring(8));
            res.Add("Email", cred["Email"].ToString().Substring(cred["Email"].ToString().IndexOf('@')));

            db.Close();

            return res;
        }
    }
}
