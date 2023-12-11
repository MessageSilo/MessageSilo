using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace MessageSilo.Features.Hubs
{
    public class SignalHub : Hub
    {
        public async Task Connect()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Guid.Empty.ToString());
            await Clients.Caller.SendAsync("connected");
        }
    }
}
