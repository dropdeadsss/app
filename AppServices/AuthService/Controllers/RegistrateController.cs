using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrateController : ControllerBase
    {
        private readonly RegistrateService service;
        public RegistrateController(RegistrateService registrateservice) {
            service = registrateservice;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] JsonElement json) 
        {
            try 
            { 
                return await service.AddUser(json, HttpContext) ? Ok("Регистрация прошла успешно.") : BadRequest("Введенны некорректные данные.");
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [AllowAnonymous]
        [HttpPost("checkemail")]
        public async Task<IActionResult> CheckEmail([FromBody] string email)
        {
            try
            {
                return await service.CheckEmail(email) ? Ok() : BadRequest("Такой адрес уже зарегистрирован");
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [AllowAnonymous]
        [HttpPost("checkusername")]
        public async Task<IActionResult> CheckUsername([FromBody] string username)
        {
            try
            {
                return await service.CheckUsername(username) ? Ok() : BadRequest("Такое имя пользователя уже зарегистрировано");
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }
    }
}
