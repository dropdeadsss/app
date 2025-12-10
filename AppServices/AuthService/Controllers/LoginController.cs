using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly LoginService service;
        public LoginController(LoginService loginservice)
        {
            service = loginservice;
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> Auth([FromBody] JsonElement json)
        {
            try
            {
                string token = await service.Auth(json);
                return token == string.Empty ? BadRequest("Неверный логин или пароль.") : Ok(token);
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }

        }

        [Authorize]
        [HttpPost("tokenauth")]
        public async Task<IActionResult> TokenAuth()
        {
            string? header = HttpContext.Request.Headers.Authorization.FirstOrDefault();
            try 
            { 
                if (header != null)
                    if (await service.TokenAuth(header))
                        return Ok();
          
                return Unauthorized();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [AllowAnonymous]
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] JsonElement json)
        {
            try
            {
                if (await service.ResetPassword(json))
                    return Ok("Пароль успешно сменен.");
                else
                    return BadRequest("Не удалось сменить пароль. Проверьте правильность введенных данных.");
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }
    }
}
