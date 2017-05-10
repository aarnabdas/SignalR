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
    public class HubConnectionD : Connection
    {
        #region private variables

        string _url = string.Empty;
        string _proxy = string.Empty;
        string _channel = string.Empty;

        HubConnection connection { get; set; }
        IHubProxy customHub { get; set; }

        #endregion private variables

        #region public constructors

        public HubConnectionD(string url)
            : this(url, "", "")
        {
        }

        public HubConnectionD(string url, string proxy, string channel)
            : base(url)
        {
            _url = url;
            _proxy = proxy;
            _channel = channel;

            this.State = ConnectionState.Disconnected;
            connection = new HubConnection(_url);

            if (!string.IsNullOrEmpty(proxy))
            {
                customHub = connection.CreateHubProxy(_proxy);
            }

            connection.Received += connection_Received;
            connection.StateChanged += connection_StateChanged;
        }

        #endregion public constructors

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
            return connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection:{0}",
                                      task.Exception.GetBaseException());
                }
                else if (!string.IsNullOrEmpty(_channel))
                {
                    customHub.Invoke<string>("Subscribe", _channel).ContinueWith(subTask =>
                    {
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
                else
                {
                    this.ChangeState(ConnectionState.Connecting, ConnectionState.Connected);
                }
            });
        }

        protected override void OnMessageReceived(Newtonsoft.Json.Linq.JToken message)
        {
            Console.WriteLine(message.ToString());
        }

        public override Task Send(string data)
        {
            return customHub.Invoke<string>("Send", data).ContinueWith(task =>
            {
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
