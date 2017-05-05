using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNet.SignalR.Client.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Client
{
    public class HubConnectionD: Connection
    {
        HubConnection connection { get; set; }
        IHubProxy customHub { get; set; }

        public HubConnectionD(string url)
            : base(url)
        {
            this.State = ConnectionState.Disconnected;
            connection = new HubConnection(url);
            customHub = connection.CreateHubProxy("messageHub");

            connection.Received += connection_Received;
            connection.StateChanged +=connection_StateChanged;
        }

        private void connection_StateChanged(StateChange obj)
        {
            this.ChangeState(obj.NewState, this.State);
        }

        private void connection_Received(string obj)
        {
            Console.WriteLine(obj);
        }

        public override Task Start(IHttpClient httpClient)
        {
            return Start();
        }

        public override Task Start(IClientTransport transport)
        {
            return Start();
        }

        public override Task Start()
        {
            this.ChangeState(ConnectionState.Disconnected, ConnectionState.Connecting);
            return connection.Start().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection:{0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    //  $.connection.hub.createHubProxy('messageHub').invoke('subscribe','Team A');
                    customHub.Invoke<string>("Subscribe", "Team A").ContinueWith(subTask => {
                        if (subTask.IsFaulted)
                        {
                            Console.WriteLine("Exception occurred during subscription: {0}", subTask.Exception.GetBaseException());
                        }
                        else
                        {
                            this.ChangeState(ConnectionState.Connecting, ConnectionState.Connected);
                        }
                    });
                }
            });
        }

        protected override void OnMessageReceived(Newtonsoft.Json.Linq.JToken message)
        {
            Console.WriteLine(message.ToString());
        }

        public override Task Send(string data)
        {
            //Thread.Sleep(500);
            return customHub.Invoke<string>("Send", data).ContinueWith(task => {
                 if (task.IsFaulted)
                 {
                     Console.WriteLine("There was an error calling send: {0}",
                                       task.Exception.GetBaseException());
                 }
             });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                connection.Stop();
            }
        }
    }
}
