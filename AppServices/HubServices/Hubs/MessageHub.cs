using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver.Core.Connections;
using System.Text.Json;

namespace HubServices.Hubs
{
    public class MessageHub : Hub
    {
        public async Task JoinChat(string channelName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channelName);
        }
        public async Task LeaveChat(string channelName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelName);
        }

        public async Task SendMessage(string channelName, JsonElement message)
        {
            await Clients.GroupExcept(channelName, Context.ConnectionId).SendAsync("ReceiveMessage", message);
        }

        public async Task IsTyping(string channelName, string user)
        {
            await Clients.GroupExcept(channelName, Context.ConnectionId).SendAsync("UserTyping", user);
        }
    }
}
