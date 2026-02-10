using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AirRouteManagementSystem.Utilities
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendPrivateMessage(string receiverUserId, string message)
        {
            var senderUserId = Context.UserIdentifier;

            await Clients.User(receiverUserId)
                .SendAsync("ReceivePrivateMessage", senderUserId, message);
        }
    }
}