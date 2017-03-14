using System;
using Microsoft.AspNet.SignalR;

namespace StautApi.SignalR
{
    public class ChatHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(string senderName, DateTime time, string message)
        {
            Clients.All.broadcastMessage(senderName, time, message);
        }
    }
}