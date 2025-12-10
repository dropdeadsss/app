using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver.Core.Connections;

namespace HubServices.Hubs
{
    public class VoiceChatHub : Hub
    {
        public async Task JoinChannel(string channelName, string WebSocketId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channelName);
            await Clients.GroupExcept(channelName, Context.ConnectionId).SendAsync("UserJoined", WebSocketId, Context.ConnectionId);
        }

        public async Task LeaveChannel(string channelName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelName);
            await Clients.GroupExcept(channelName, Context.ConnectionId).SendAsync("UserLeft", Context.ConnectionId);
        }

        public async Task WebSocketResponse(string ConnectionId, string WebSocketId)
        {
            await Clients.Clients(ConnectionId).SendAsync("WebSocketResponse", WebSocketId);
        }
    }
}