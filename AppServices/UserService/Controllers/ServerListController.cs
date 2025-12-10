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
    public class ServerListController : ControllerBase
    {
        private readonly ServerListService service;
        public ServerListController(ServerListService userinfoservice)
        {
            service = userinfoservice;
        }

        [Authorize]
        [HttpPost("getserverlist")]
        public async Task<IActionResult> GetServerList([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.GetServerList(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("addtoserverlist")]
        public async Task<IActionResult> AddToServerList([FromBody] JsonElement json)
        {
            try
            {
                return await service.AddToServerList(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("removefromserverlist")]
        public async Task<IActionResult> RemoveFromServerList([FromBody] JsonElement json)
        {
            try
            {
                return await service.RemoveFromServerList(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }
    }
}
