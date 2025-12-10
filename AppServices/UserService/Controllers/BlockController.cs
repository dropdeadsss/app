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
    public class BlockController : ControllerBase
    {
        private readonly BlockService service;
        public BlockController(BlockService userinfoservice)
        {
            service = userinfoservice;
        }

        [Authorize]
        [HttpPost("block")]
        public async Task<IActionResult> BlockUser([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.BlockUser(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("unblock")]
        public async Task<IActionResult> UnblockUser([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.UnblockUser(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("checkblock")]
        public async Task<IActionResult> CheckBlock([FromBody] JsonElement json)
        {
            try
            {
                var res = await service.CheckBlock(json);
                return res == string.Empty ? Ok() : BadRequest(res.ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));

            }
        }

        [Authorize]
        [HttpPost("getblocked")]
        public async Task<IActionResult> GetBlocked([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.GetBlocked(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));

            }
        }
    }
}