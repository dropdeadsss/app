using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserService.Services;
using System.Text;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddTransient<UserInfoService>();
            builder.Services.AddTransient<BlockService>();
            builder.Services.AddTransient<FriendService>();
            builder.Services.AddTransient<SettingsService>();
            builder.Services.AddTransient<ServerListService>();

            var jwtkey = builder.Configuration["Token:Key_1"];
            var jwtbytes = Encoding.UTF8.GetBytes(jwtkey);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["TokenParams:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["TokenParams:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtbytes),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
