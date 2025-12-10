using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UserServersService.Models;
using UserServersService.Services;

namespace UserServersService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerControlController : ControllerBase
    {
        private readonly ServerControlService service;
        public ServerControlController(ServerControlService serverservice)
        {
            service = serverservice;
        }



    }
}




/*
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddFriend([FromBody] JsonElement json)
        {
            try
            {
                return Ok();
            }
            catch
            {
                return StatusCode(503,Errors.GetError(503));
            }
        }
 */