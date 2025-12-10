using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Text.Json;
using UserServersService.Models;
using UserServersService.Services;

namespace UserServersService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private readonly ServerService service;
        public ServerController(ServerService serverservice)
        {
            service = serverservice;
        }

        [Authorize]
        [HttpPost("getserver")]
        public async Task<IActionResult> GetServer([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.GetServer(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateServer([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.CreateServer(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteServer([FromBody] JsonElement json)
        {
            try
            {
                return await service.DeleteServer(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("changename")]
        public async Task<IActionResult> ChangeName([FromBody] JsonElement json)
        {
            try
            {
                return await service.ChangeName(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("changeimg")]
        public async Task<IActionResult> ChangeImgAndIcon([FromBody] JsonElement json)
        {
            try
            {
                return await service.ChangeImgAndIcon(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("changedescription")]
        public async Task<IActionResult> ChangeDescription([FromBody] JsonElement json)
        {
            try
            {
                return await service.ChangeDescription(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }
    }
}
