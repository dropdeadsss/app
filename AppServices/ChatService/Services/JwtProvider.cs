using ChatService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatService.Services
{
    public static class JwtProvider
    {
        public static List<Claim> ReadToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            List<Claim> claims = tokenHandler.ReadJwtToken(token).Claims.ToList();
            return claims;
        }

        public static async Task<bool> ValidateJwt(string token, IConfiguration configuration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            TokenValidationParameters validation = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["TokenParams:Issuer"],

                ValidateAudience = true,
                ValidAudience = configuration["TokenParams:Audience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key_1"])),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var validatedToken = await tokenHandler.ValidateTokenAsync(token, validation);

            return validatedToken.IsValid;
        }
    }
}
