using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace UserServersService.Services
{
    public class ServerService
    {
        private readonly IConfiguration config;
        public ServerService(IConfiguration configuration)
        {
            config = configuration;
        }

        public async Task<BsonDocument> GetServer(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "userid" }))
                return new BsonDocument();

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var servers = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            var filter = Builders<BsonDocument>.Filter.And(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString())), Builders<BsonDocument>.Filter.ElemMatch("Members", Builders<BsonDocument>.Filter.Eq("Users", ObjectId.Parse(dict["userid"].ToString()))));

            if (await servers.Find(filter).AnyAsync())
            {
                return await servers.Find(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(dict["_id"].ToString()))).FirstOrDefaultAsync();
            }

            return new BsonDocument();
        }

        public async Task<BsonDocument> CreateServer(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "name" }))
               return new BsonDocument();

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var servers = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            BsonDocument ServerOwner = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name", "Управление сервером"},
                {"Description", "Позволяет делать все" }
            };

            BsonDocument ServerAdminOrModer = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name", "Доступ ко всем каналам и чатам"},
                {"Description", "Позволяет использовать любые каналы и чаты" }
            };

            BsonDocument RuleServer = new BsonDocument
            {        
                {"_id", ObjectId.GenerateNewId()},
                {"Name", "Управление сервером"},
                {"Description", "Позволяет менять название, описание и иконку сервера" }          
            };

            BsonDocument RuleRoles = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name", "Управление ролями"},
                {"Description", "Позволяет создавать, менять, удалять и назначать роли (кроме администратора и создателя)" }
            };

            BsonDocument RuleChannels = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name", "Управление каналами"},
                {"Description", "Позволяет создавать, удалять, менять и изменять доступ каналам" }
            };

            BsonDocument RuleUsers = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name", "Управление участиками"},
                {"Description", "Позволяет блокировать и разблокировать пользователей, удалять сообщения из чата, выгонять пользователей из голосовго канала" }
            };

            BsonDocument CommonUser = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name", "Обычный участник"},
                {"Description", "Позволяет использовать открытые чаты и голосовые каналы" }
            };

            BsonDocument owner = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name",  "Создатель"},
                {"Description", "Создатель сервера с неограниченной властью"},
                {"Level", 0},
                {"Users", new BsonArray {ObjectId.Parse(dict["_id"].ToString())}},
                {"Permissions", new BsonArray {ServerOwner}},
                {"DateCreated", DateTime.Now},
                {"DateModified", DateTime.Now}
            };

            BsonDocument admin = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name",  "Администратор"},
                {"Description", "Заместитель создателя. Может все, кроме изменения данных сервера и управления создателем"},
                {"Level", 1},
                {"Users", new BsonArray()},
                {"Permissions", new BsonArray { RuleServer, RuleRoles, RuleChannels, RuleUsers, ServerAdminOrModer }},
                {"DateCreated", DateTime.Now},
                {"DateModified", DateTime.Now}
            };

            BsonDocument moderator = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name",  "Модератор"},
                {"Description", "Роль позволяющая удалять сообщения и блокировать пользователей"},
                {"Level", 2},
                {"Users", new BsonArray()},
                {"Permissions", new BsonArray { RuleRoles, RuleChannels, RuleUsers, ServerAdminOrModer }},
                {"DateCreated", DateTime.Now},
                {"DateModified", DateTime.Now}
            };

            BsonDocument participant = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name",  "Участник"},
                {"Description", "Базовая роль для всех присоединившихся к серверу"},
                {"Level", 10},
                {"Users", new BsonArray()},
                {"Permissions", new BsonArray { CommonUser }},
                {"DateCreated", DateTime.Now},
                {"DateModified", DateTime.Now}
            };

            BsonDocument ChatChannel = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name",  "Основной"},
                {"Permissions", new BsonArray { owner["_id"], admin["_id"], moderator["_id"], participant["_id"] }},
                {"DateCreated", DateTime.Now},
                {"DateModified", DateTime.Now}
            };

            BsonDocument VoiceChannel = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name",  "Основной"},
                {"Permissions", new BsonArray { owner["_id"], admin["_id"], moderator["_id"], participant["_id"] }},
                {"DateCreated", DateTime.Now},
                {"DateModified", DateTime.Now}
            };

            BsonDocument document = new BsonDocument
            {
                {"_id", ObjectId.GenerateNewId()},
                {"Name",  dict["name"].ToString()},
                {"ChatChannels", new BsonArray{ ChatChannel }},
                {"VoiceChannels", new BsonArray{ VoiceChannel }},
                {"Members",  new BsonArray {owner, admin, moderator, participant }},
                {"DateCreated", DateTime.Now},
                {"DateModified", DateTime.Now}
            };

            await servers.InsertOneAsync(document);

            return document;
        }

        public async Task<bool> DeleteServer(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "_id", "userid" }))
                return false;

            MongoDB db = new MongoDB(config);
            var dbcontext = db.Open();

            var servers = dbcontext.GetCollection<BsonDocument>(config["Collections:Col_1"]);

            if (await servers.Find(Builders<BsonDocument>.Filter.ElemMatch("Members", Builders<BsonDocument>.Filter.And(Builders<BsonDocument>.Filter.Eq("Users", dict["userid"].ToString()), Builders<BsonDocument>.Filter.Eq("Role", "Создатель")))).AnyAsync())
            { 
                await servers.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", dict["_id"].ToString()), Builders<BsonDocument>.Update.Set("DateDeleted", DateTime.Now)); 
                return true;
            }
            else
                return false;
        }

        public async Task<bool> ChangeName(JsonElement json)
        {
            return true;
        }

        public async Task<bool> ChangeImgAndIcon(JsonElement json)
        {
            return true;
        }

        public async Task<bool> ChangeDescription(JsonElement json)
        {
            return true;
        }
    }
}