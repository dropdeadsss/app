using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Text.Json;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly SettingsService service;
        public SettingsController(SettingsService userinfoservice)
        {
            service = userinfoservice;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveSettings([FromBody] JsonElement json)
        {
            try
            {
                return await service.SaveSettings(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetSettings([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.GetSettings(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

    }
}
