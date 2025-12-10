using AuthService.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using UserService.Services;

namespace AuthService.Services
{
    public class LoginService
    {
        private readonly IConfiguration configuration;
        public LoginService(IConfiguration conf)
        {
            configuration = conf;
        }
        public async Task<string> Auth(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "email", "password" }))
                return string.Empty;

            Credentials credentials = new Credentials() { Email = dict["email"].ToString(), Password = await HashProvider.GetHash(dict["password"].ToString()) };

            MongoDB db = new MongoDB(configuration);
            var dbcontext = db.Open();

            var creds = dbcontext.GetCollection<Credentials>(configuration["Collections:Col_1"]);
            var users = dbcontext.GetCollection<User>(configuration["Collections:Col_2"]);

            if(await creds.Find(x => x.Email == credentials.Email && x.Password == credentials.Password).AnyAsync())
            {
                var idProjection = Builders<Credentials>.Projection.Include(x => x.Id);
                var nameProjection = Builders<User>.Projection.Include(x => x.Username).Exclude(x => x.Id);

                var id = await creds.Find(x => x.Email == credentials.Email && x.Password == credentials.Password).Project<BsonDocument>(idProjection).FirstOrDefaultAsync();
                var name = await users.Find(x => x.Id == id[0]).Project<BsonDocument>(nameProjection).FirstOrDefaultAsync();

                string token = JwtProvider.CreateToken(id[0].ToString(), name[0].ToString(), configuration);

                db.Close();

                return token;
            }
            else
            {
                db.Close();

                return string.Empty;
            }

        }

        public async Task<bool> TokenAuth(string token)
        {
            return await JwtProvider.ValidateJwt(token, configuration);
        }

        public async Task<bool> ResetPassword(JsonElement json)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "email", "password" }))
                return false;

            MongoDB db = new MongoDB(configuration);
            var dbcontext = db.Open();

            var creds = dbcontext.GetCollection<Credentials>(configuration["Collections:Col_1"]);

            var update = Builders<Credentials>.Update.Set("Password", HashProvider.GetHash(dict["password"].ToString()));

            if (await creds.Find(x => x.Email == dict["email"].ToString()).AnyAsync())
            {
                await creds.UpdateOneAsync(x => x.Email == dict["email"].ToString(), update);
                return true;
            } 
            else 
                return false;
        }
    }
}
