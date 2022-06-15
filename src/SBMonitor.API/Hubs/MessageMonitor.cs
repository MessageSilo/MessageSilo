using Microsoft.AspNetCore.SignalR;
using SBMonitor.Core.Interfaces;

namespace SBMonitor.API.Hubs
{
    public class MessageMonitor : Hub, IMessageMonitor
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
