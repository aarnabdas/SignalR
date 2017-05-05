using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SignalRSample
{
    public class ChatHub : Hub
    {
        public void Send(string message)
        {
            var messageItem = JsonConvert.DeserializeObject<Models.MessageItem>(message);
            Clients.All.addNewMessageToPage(messageItem.Name, messageItem.Message);
        }

        public override Task OnConnected()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }
}