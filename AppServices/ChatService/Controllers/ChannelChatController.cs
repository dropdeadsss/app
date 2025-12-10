using ChatService.Models;
using ChatService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Text.Json;

namespace ChatService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelChatController : ControllerBase
    {
        private readonly ChannelChatService service;
        public ChannelChatController(ChannelChatService userinfoservice)
        {
            service = userinfoservice;
        }

        [Authorize]
        [HttpPost("getmessages")]
        public async Task<IActionResult> GetMessages([FromBody] JsonElement json)
        {
            try
            {
                return Ok((await service.GetMessages(json)).ToJson());
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }


        [Authorize]
        [HttpPost("sendmessage")]
        public async Task<IActionResult> SendMessage([FromBody] JsonElement json)
        {
            try
            {
                return await service.SendMessage(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("sendvoicemessage")]
        public async Task<IActionResult> SendVoiceMessage([FromBody] JsonElement json)
        {
            try
            {
                return await service.SendVoiceMessage(json) ? Ok() : BadRequest();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }



        [Authorize]
        [HttpPost("updatemessage")]
        public async Task<IActionResult> UpdateMessage([FromBody] JsonElement json)
        {
            try
            {
                return Ok();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }

        [Authorize]
        [HttpPost("deletemessage")]
        public async Task<IActionResult> DeleteMessage([FromBody] JsonElement json)
        {
            try
            {
                return Ok();
            }
            catch
            {
                return StatusCode(503, Errors.GetError(503));
            }
        }
    }
}
