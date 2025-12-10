
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System.Data;

namespace AuthService.Services
{
    public class MongoDB
    {
        private readonly string connectionstring;
        private readonly string databaseContext;
        private IMongoDatabase database;
        private MongoClient db;
        public MongoDB(IConfiguration configuration) {
            connectionstring = configuration.GetConnectionString("MongoDB");
            databaseContext = configuration["Databases:Database_1"];
        }
        public IMongoDatabase Open()
        {
            db = new MongoClient(connectionstring);
            database = db.GetDatabase(databaseContext);
            return database;
        }
        public void Close()
        {
            db.Dispose();
        }
    }
}
