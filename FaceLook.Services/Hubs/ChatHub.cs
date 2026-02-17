using Microsoft.AspNetCore.SignalR;

namespace FaceLook.Services.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        public async Task SendMessageAsync(string sender, string message)
        {
            await Clients.All.ReceiveMessage(sender, message);
        }

        public async Task SendMessageToGroupAsync(string sender, string receiver, string message)
        {
            await Clients.Group(receiver).ReceiveMessage(sender, message);
        }

        public async Task JoinGroupAsync(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        public async Task LeaveGroupAsync(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }
    }
}