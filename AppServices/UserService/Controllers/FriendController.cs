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
    public class FriendController : ControllerBase
    {

        private readonly FriendService service;
        public FriendController(FriendService userinfoservice)
        {
            service = userinfoservice;
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddFriend([FromBody] JsonElement json)
        {
            try
            {               
                return Ok((await service.AddFriend(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFriend([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.RemoveFriend(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.AcceptFriendRequest(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("deny")]
        public async Task<IActionResult> DenyFriendRequest([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.DenyFriendRequest(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("check")]
        public async Task<IActionResult> CheckFriend([FromBody] JsonElement json)
        {
            try
            {
                var res = await service.CheckFriend(json);
                return res == string.Empty ? Ok() : BadRequest(res.ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("Getfriends")]
        public async Task<IActionResult> GetFriends([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.GetFriends(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("Getreqeusts")]
        public async Task<IActionResult> GetReqeusts([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.GetReqeusts(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("Getinvites")]
        public async Task<IActionResult> GetInvites([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.GetInvites(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }
    }
}
