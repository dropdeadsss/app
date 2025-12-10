using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly UserInfoService service;
        public UserInfoController(UserInfoService userinfoservice) {
            service = userinfoservice;
        }

        [Authorize]
        [HttpPost("getselfuserinfo")]
        public async Task<IActionResult> GetSelfUserInfo([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.GetSelfUserInfo(HttpContext.Request.Headers.Authorization, json)).ToJson());
            }
            catch {
                return StatusCode(503,Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("userinfo")]
        public async Task<IActionResult> GetUserInfo([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.GetUserInfo(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }  
        }

        [Authorize]
        [HttpPost("changenickname")]
        public async Task<IActionResult> ChangeNickname([FromBody] JsonElement json)
        {
            try
            {
                return await service.ChangeNickname(json) ? Ok() : BadRequest();
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

        [Authorize]
        [HttpPost("changeprofileimg")]
        public async Task<IActionResult> ChangeProfileImg([FromBody] JsonElement json)
        {
            try
            {
                return await service.ChangeProfileImg(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
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