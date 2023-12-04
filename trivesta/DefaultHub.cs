using Microsoft.AspNetCore.SignalR;

namespace trivesta
{
    public class DefaultHub : Hub
    {
        public async Task JoinRoom(string roomID, string userID, string username)
        {
            //HubUser._data.Add(Context.ConnectionId, userID);
            //HubUser._data.Add(userID, username);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomID);
            await Clients.Group(roomID).SendAsync("user-connected", userID, username);
            //await Clients.All.SendAsync("ReceiveMessage", "user", "message");
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Clients.All.SendAsync("user-disconnected");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
