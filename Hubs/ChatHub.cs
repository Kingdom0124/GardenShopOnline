using Microsoft.AspNet.SignalR;

namespace GardenShopOnline.Hubs
{
    public class ChatHub : Hub
    {
        public static void Send(string time, string message, string connectionId, string userId, string toUserId, string Img)
        {
            // Call the addNewMessageToPage method to update clients.
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.AllExcept(connectionId).addNewMessageToPage(time, message, userId, toUserId, Img);
        }
    }
}