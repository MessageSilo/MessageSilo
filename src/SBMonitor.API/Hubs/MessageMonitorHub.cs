using Microsoft.AspNetCore.SignalR;
using SBMonitor.Core.Interfaces;

namespace SBMonitor.API.Hubs
{
    public class MessageMonitorHub : Hub, IMessageMonitor
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Groups.AddToGroupAsync(Context.ConnectionId, Guid.Empty.ToString());
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Guid.Empty.ToString());
        }
    }
}
