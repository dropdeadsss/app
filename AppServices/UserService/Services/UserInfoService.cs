using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using UserService.Models;
using UserService.Services;

namespace UserService.Services
{
    public class UserInfoService
    {
        private readonly IConfiguration config; 
        public UserInfoService(IConfiguration configuration) {
            config = configuration;
        }

        public async Task<BsonDocument> GetSelfUserInfo(string? token, JsonElement json)
        {
            if(token  == null || token == string.Empty)
                return new BsonDocument();

            token = token.Substring(7);

            if(!await JwtProvider.ValidateJwt(token, config))
                return new BsonDocument();

            var claims = JwtProvider.ReadToken(token);

            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "nickname" }))
                dict.Add("nickname", "user");

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "createdat" }))
                dict.Add("createdat", DateTime.Now);

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();
            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var filter = Builders<BsonDocument>.Filter.Eq("Username", claims[1].Value);

            var user = await users.Find(filter).FirstOrDefaultAsync();

            if (user != null)
            {
                var projection = Builders<BsonDocument>.Projection.Include("Username").Include("Nickname").Include("_id").Include("Description").Include("ProfileImg").Include("CreatedAt");

                if (user.Contains("Friends"))
                {
                    user["Friends"] = BsonArray.Create(await users.Find(Builders<BsonDocument>.Filter.In("_id", user.GetValue("Friends").AsBsonArray)).Project(projection).ToListAsync());
                }
                
                if (user.Contains("Blocked"))
                {
                    user["Blocked"] = BsonArray.Create(await users.Find(Builders<BsonDocument>.Filter.In("_id", user.GetValue("Blocked").AsBsonArray)).Project(projection).ToListAsync());
                }

                if (user.Contains("FriendInvites"))
                {
                    user["FriendInvites"] = BsonArray.Create(await users.Find(Builders<BsonDocument>.Filter.In("_id", user.GetValue("FriendInvites").AsBsonArray)).Project(projection).ToListAsync());
                }

                if (user.Contains("FriendRequests"))
                {
                    user["FriendRequests"] = BsonArray.Create(await users.Find(Builders<BsonDocument>.Filter.In("_id", user.GetValue("FriendRequests").AsBsonArray)).Project(projection).ToListAsync());
                }

                db.Close();

                return user;
            }
            else
            {
                BsonDocument bson = new BsonDocument();
                bson.Add(new BsonElement("_id", ObjectId.GenerateNewId()));
                bson.Add(new BsonElement("Username", claims[1].Value));
                bson.Add(new BsonElement("Nickname", dict["nickname"].ToString()));
                bson.Add(new BsonElement("DateModified", DateTime.Now));
                bson.Add(new BsonElement("CreatedAt", DateTime.Parse(dict["createdat"].ToString())));
                await users.InsertOneAsync(bson);

                db.Close();

                return bson;
            }          
        }

        public async Task<List<BsonDocument>> GetUserInfo(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id" }))
                return new List<BsonDocument>();

            JsonElement arr = (JsonElement)dict["_id"];
            List<ObjectId> ids = new List<ObjectId>();
            if(arr.ValueKind == JsonValueKind.Array)
            {
                foreach(var x in arr.EnumerateArray())
                {
                    ids.Add(ObjectId.Parse(x.ToString()));
                }
            }
            else
            {
                return new List<BsonDocument>();
            }

                MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();
            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var projection = Builders<BsonDocument>.Projection.Include("Username").Include("Nickname").Include("_id").Include("Description").Include("ProfileImg").Include("CreatedAt");
            var filter = Builders<BsonDocument>.Filter.In("_id", BsonArray.Create(ids));

            var res = await users.Find(filter).Project(projection).ToListAsync();

            db.Close();

            return res ?? new List<BsonDocument>();
        }

        public async Task<bool> ChangeProfileImg(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "profileimg" }))
                return false;

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();
            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var res = await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Set("ProfileImg", dict["profileimg"].ToString()).Set("DateModified", DateTime.Now));

            db.Close();

            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<bool> ChangeUsername(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "username" }))
                return false;

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();
            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var res = await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Set("Username", dict["username"].ToString()).Set("DateModified", DateTime.Now));

            db.Close();

            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<bool> ChangeNickname(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "nickname" }))
                return false;

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();
            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var res = await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Set("Nickname", dict["nickname"].ToString()).Set("DateModified", DateTime.Now));

            db.Close();

            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

        public async Task<bool> ChangeDescription(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "description" }))
                return false;

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();
            var users = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var res = await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Update.Set("Description", dict["description"].ToString()).Set("DateModified", DateTime.Now));

            db.Close();

            return res.IsAcknowledged && res.ModifiedCount > 0;
        }

    }
}
