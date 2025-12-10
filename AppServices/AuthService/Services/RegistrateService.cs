using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System.Net;
using System.Net.Mail;
using System.Numerics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Transactions;
using UserService.Services;

namespace AuthService.Services
{
    public class RegistrateService
    {
        private readonly IConfiguration configuration;

        public RegistrateService(IConfiguration conf)
        {
            configuration = conf;
        }
        public async Task<bool> AddUser(JsonElement json, HttpContext http)
        {
            var dict = CollectionOperations.JsonToDictionary(json);

            if (!CollectionOperations.ValidateDictionary<string, object>(dict, new List<string>() { "username", "email", "phone", "dob", "password" }))
                return false;

            if (!DataValidators.ValidateEmail(dict["email"].ToString()) || !DataValidators.ValidatePhone(dict["phone"].ToString()))
                return false;

            if (await CheckUsername(dict["username"].ToString()) || await CheckEmail(dict["email"].ToString()))
                return false;

            dict["password"] = await HashProvider.GetHash(dict["password"].ToString());

            string Ip = http.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? http.Connection.RemoteIpAddress?.ToString();

            var ad = Dns.GetHostEntry(Ip);

            List<string> addresses = new List<string>();

            foreach (var address in (await Dns.GetHostEntryAsync(Ip)).AddressList) 
            {
                addresses.Add(address.ToString());
            }

            MongoDB db = new MongoDB(configuration);
            var dbcontext = db.Open();

            var creds = dbcontext.GetCollection<Credentials>(configuration["Collections:Col_1"]);
            var users = dbcontext.GetCollection<User>(configuration["Collections:Col_2"]);
            var devices = dbcontext.GetCollection<DeviceInfo>(configuration["Collections:Col_3"]);

            ObjectId id = ObjectId.GenerateNewId();

            Credentials credentials = new Credentials() { Id = id, Email = dict["email"].ToString(), Password = dict["password"].ToString(), DateModified = DateTime.Now };
            DeviceInfo deviceInfo = new DeviceInfo() { Id = id, Ip = addresses, DateModified = DateTime.Now };
            User user = new User() { Id = id, Phone = dict["phone"].ToString(), Username = dict["username"].ToString(), Dob = DateOnly.Parse(dict["dob"].ToString()), DateModified = DateTime.Now , DateCreated = DateTime.Now };

            await creds.InsertOneAsync(credentials);
            await users.InsertOneAsync(user);
            await devices.InsertOneAsync(deviceInfo);

            db.Close();

            return true;
        }

        public async Task<bool> CheckUsername(string username)
        {
            MongoDB db = new MongoDB(configuration);

            var dbcontext = db.Open();
            var users = dbcontext.GetCollection<User>(configuration["Collections:Col_2"]);
            bool res = await users.Find(x => x.Username == username).AnyAsync();

            db.Close();

            return res;
        }

        public async Task<bool> CheckEmail(string email)
        {
            MongoDB db = new MongoDB(configuration);

            var dbcontext = db.Open();
            var creds = dbcontext.GetCollection<Credentials>(configuration["Collections:Col_1"]);
            bool res = await creds.Find(x => x.Email == HashProvider.GetHash(email).ToString()).AnyAsync();

            db.Close();

            return res;
        }
    }
}