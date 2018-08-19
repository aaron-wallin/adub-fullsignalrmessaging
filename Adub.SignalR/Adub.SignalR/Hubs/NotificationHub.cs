using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Adub.SignalR.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Console.Out.WriteLineAsync($"Processing message from {user} : {message}");
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await Console.Out.WriteLineAsync("DISCONNECTED: " + exception.ToString());
            await base.OnDisconnectedAsync(exception);
        }
    }
}
