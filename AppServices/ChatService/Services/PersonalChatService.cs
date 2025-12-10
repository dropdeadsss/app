using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatService.Services
{
    public class PersonalChatService
    {
        private readonly IConfiguration config;
        public PersonalChatService(IConfiguration configuration)
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

            var chats = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            int limit = 50;
            int offset = 50 * Int32.Parse(dict["offset"].ToString());

            var filter = Builders<BsonDocument>.Filter.Or(Builders<BsonDocument>.Filter.And(Builders<BsonDocument>.Filter.Eq("From", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Filter.Eq("To", ObjectId.Parse(dict["userid"].ToString()))), 
                                                          Builders<BsonDocument>.Filter.And(Builders<BsonDocument>.Filter.Eq("From", ObjectId.Parse(dict["userid"].ToString())), Builders<BsonDocument>.Filter.Eq("To", ObjectId.Parse(dict["_id"].ToString()))));

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

            var chats = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            BsonDocument doc = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId() },
                {"From",  ObjectId.Parse(dict["_id"].ToString()) },
                {"To",  ObjectId.Parse(dict["userid"].ToString()) },
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

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "userid", "content", "isvoice", "duration" ,"datetime" }))
                return false;

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var chats = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

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

            if(bytes != null)
            {
                BsonDocument doc = new BsonDocument
                {
                    {"_id", ObjectId.GenerateNewId() },
                    {"From",  ObjectId.Parse(dict["_id"].ToString()) },
                    {"To",  ObjectId.Parse(dict["userid"].ToString()) },
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



/*public async Task<bool> SendMessage(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "userid", "content", "imgs", "files", "isvoice", "datetime" }))
                return false;

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var chats = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            if (Boolean.Parse(dict["isvoice"].ToString()))
            {
                BsonDocument doc = new BsonDocument
                {
                    {"_id", ObjectId.GenerateNewId() },
                    {"From",  ObjectId.Parse(dict["_id"].ToString()) },
                    {"To",  ObjectId.Parse(dict["_id"].ToString()) },
                    {"Content",  dict["content"].ToString() },
                    {"IsVoice", BsonBoolean.Create(false)},
                    {"DateTime", DateTime.Parse(dict["datetime"].ToString()) }
                };

                if (dict["imgs"] != null)
                {
                    List<string> imgs = dict["imgs"].ToString().Split(',').ToList();
                    doc.Add(new BsonElement("Images", new BsonArray(imgs)));
                }

                if (dict["files"] != null)
                {
                    List<string> files = dict["files"].ToString().Split(',').ToList();
                    doc.Add(new BsonElement("Files", new BsonArray(files)));
                }

                await chats.InsertOneAsync(doc);
                
                return true;
            }
            else if (Boolean.Parse(dict["isvoice"].ToString()))
            {
                JsonElement voice = (JsonElement)dict["content"];
                List<byte> bytes = new List<byte>();
                if(voice.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in voice.EnumerateArray())
                    {
                        if(item.TryGetByte(out byte b))
                            bytes.Add(b);
                    }
                }

                BsonDocument doc = new BsonDocument
                {
                    {"_id", ObjectId.GenerateNewId() },
                    {"From",  ObjectId.Parse(dict["_id"].ToString()) },
                    {"To",  ObjectId.Parse(dict["_id"].ToString()) },
                    {"Content", BsonBinaryData.Create(bytes) },
                    {"IsVoice", BsonBoolean.Create(true)},
                    {"DateTime", DateTime.Parse(dict["datetime"].ToString()) }
                };

                await chats.InsertOneAsync(doc);

                return true;
            }

            return false;
        }*/








