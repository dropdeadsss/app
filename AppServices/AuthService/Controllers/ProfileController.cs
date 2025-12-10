using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Text.Json;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService service;
        public ProfileController(ProfileService profile) 
        {
            service = profile;
        }

        
        [Authorize]
        [HttpPost("changeusername")]
        public async Task<IActionResult> ChangeUsername([FromBody] JsonElement json)
        {
            try
            {
                return await service.ChangeUsername(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("changeemail")]
        public async Task<IActionResult> ChangeEmail([FromBody] JsonElement json)
        {
            try
            {
                return await service.ChangeEmail(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("changephone")]
        public async Task<IActionResult> ChangePhone([FromBody] JsonElement json)
        {
            try
            {
                return await service.ChangePhone(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("getprofileinfo")]
        public async Task<IActionResult> GetProfileInfo([FromBody] JsonElement json)
        {
            try
            {
                return  Ok((await service.GetProfileInfo(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }
    }
}
