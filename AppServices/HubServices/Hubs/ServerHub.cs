using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver.Core.Connections;
using System.Text.Json;

namespace HubServices.Hubs
{
    public class ServerHub : Hub
    {
        public async Task JoinServer(string channelName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channelName);
            await Clients.GroupExcept(channelName, Context.ConnectionId).SendAsync("UserJoinedServer", Context.ConnectionId);
        }
        public async Task LeaveServer(string channelName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelName);
        }      
        public async Task JoinServerResponse(string ConnectionId, JsonElement user)
        {
            await Clients.Client(ConnectionId).SendAsync("UserJoinedServerResponse", user);
        }
        public async Task ChannelChange(string channelName, JsonElement user, string? from, string? to)
        {
            await Clients.GroupExcept(channelName, Context.ConnectionId).SendAsync("ChannelChanged", user, from, to);
        }
    }
}