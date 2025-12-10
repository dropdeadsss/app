using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;

namespace UserServersService.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerControlService : ControllerBase
    {
        private readonly IConfiguration configuration;
        public ServerControlService(IConfiguration config)
        {
            configuration = config;
        }

        public async Task<bool> Test()
        {
            MongoDB db = new MongoDB(configuration);

            var dbcontext = db.Open();

            var users = dbcontext.GetCollection<BsonDocument>(configuration["Collections:Col_2"]);

            return true;
        }
    }
}
