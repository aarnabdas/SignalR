using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace SignalRSample
{
    public class ChatHub : Hub
    {
        public void Send(string message)
        {
            var messageItem = JsonConvert.DeserializeObject<Models.MessageItem>(message);
            Clients.All.addNewMessageToPage(messageItem.Name, messageItem.Message);            
        }
    }
}