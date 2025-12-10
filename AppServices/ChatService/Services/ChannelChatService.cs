using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace ChatService.Services
{
    public class ChannelChatService
    {
        private readonly IConfiguration config;
        public ChannelChatService(IConfiguration configuration)
        {
            config = configuration;
        }

        public async Task<List<BsonDocument>> GetMessages(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "userid", "offset" }))
                return new List<BsonDocument>();

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var chats = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_2"]);

            int limit = 50;
            int offset = 50 * Int32.Parse(dict["offset"].ToString());

            var filter = Builders<BsonDocument>.Filter.Eq("ChannelId", ObjectId.Parse(dict["_id"].ToString()));

            var sort = Builders<BsonDocument>.Sort.Ascending("DateTime");

            var res = await chats.Find(filter).Limit(limit).Skip(offset).Sort(sort).ToListAsync();

            return res;
        }

        public async Task<bool> SendMessage(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "userid", "content", "imgs", "files", "isvoice", "datetime" }))
                return false;

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var chats = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_2"]);

            BsonDocument doc = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId() },
                {"ChannelId",  ObjectId.Parse(dict["_id"].ToString()) },
                {"From",  ObjectId.Parse(dict["userid"].ToString()) },
                {"Content",  dict["content"].ToString() },
                {"IsVoice", BsonBoolean.Create(false)},
                {"DateTime", DateTime.Parse(dict["datetime"].ToString()) }
            };

            JsonElement images = (JsonElement)dict["imgs"];
            JsonElement fls = (JsonElement)dict["files"];

            if (images.ValueKind == JsonValueKind.Array && images.EnumerateArray().Count() > 0)
            {
                List<string> imgs = dict["imgs"].ToString().Split(',').ToList();
                doc.Add(new BsonElement("Images", new BsonArray(imgs)));
            }

            if (fls.ValueKind == JsonValueKind.Array && fls.EnumerateArray().Count() > 0)
            {
                List<string> files = dict["files"].ToString().Split(',').ToList();
                doc.Add(new BsonElement("Files", new BsonArray(files)));
            }

            await chats.InsertOneAsync(doc);

            return true;
        }

        public async Task<bool> SendVoiceMessage(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "userid", "content", "isvoice", "duration", "datetime" }))
                return false;

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var chats = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_2"]);

            JsonElement voice = (JsonElement)dict["content"];
            byte[]? bytes = null;
            if (voice.ValueKind == JsonValueKind.Array)
            {
                bytes = new byte[voice.EnumerateArray().Count()];
                int i = 0;
                foreach (var item in voice.EnumerateArray())
                {
                    if (item.TryGetByte(out byte b))
                    {
                        bytes[i] = b;
                        i++;
                    }
                }
            }

            if (bytes != null)
            {
                BsonDocument doc = new BsonDocument
                {
                    {"_id", ObjectId.GenerateNewId() },
                    {"ChannelId",  ObjectId.Parse(dict["_id"].ToString()) },
                    {"From",  ObjectId.Parse(dict["userid"].ToString()) },
                    {"Content", BsonBinaryData.Create(bytes) },
                    {"IsVoice", BsonBoolean.Create(true) },
                    {"Duration", Int32.Parse(dict["duration"].ToString()) },
                    {"DateTime", DateTime.Parse(dict["datetime"].ToString()) }
                };

                await chats.InsertOneAsync(doc);

                return true;
            }

            else
                return false;
        }
    }
}
