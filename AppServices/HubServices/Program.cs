using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.IdentityModel.Tokens;
using HubServices.Hubs;
using System.Text;

namespace HubServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();

            var jwtkey = builder.Configuration["Token:Key_1"];
            var jwtbytes = Encoding.UTF8.GetBytes(jwtkey);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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

            app.MapHub<VoiceChatHub>("/voicechat", options =>
            {
                options.ApplicationMaxBufferSize = 102400;
                options.TransportMaxBufferSize = 102400;
                options.LongPolling.PollTimeout = TimeSpan.FromMinutes(1);
                options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
            });

            app.MapHub<ServerHub>("/server", options =>
            {
                options.ApplicationMaxBufferSize = 102400;
                options.TransportMaxBufferSize = 102400;
                options.LongPolling.PollTimeout = TimeSpan.FromMinutes(1);
                options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
            });

            app.MapHub<MessageHub>("/chat", options =>
            {
                options.ApplicationMaxBufferSize = 102400;
                options.TransportMaxBufferSize = 102400;
                options.LongPolling.PollTimeout = TimeSpan.FromMinutes(1);
                options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
            });

            app.MapHub<UserStatusHub>("/userstatus", options =>
            {
                options.ApplicationMaxBufferSize = 102400;
                options.TransportMaxBufferSize = 102400;
                options.LongPolling.PollTimeout = TimeSpan.FromMinutes(1);
                options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();
        }
    }
}
