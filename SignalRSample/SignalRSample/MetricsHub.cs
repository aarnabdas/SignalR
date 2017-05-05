using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Web;

namespace SignalRSample
{
    public class MetricsHub : Hub
    {
        public MetricsHub()
            : base()
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += (sender, e) => HandleTimer();
            timer.Start();
        }

        private void HandleTimer()
        {
            Clients.All.SendCurrentMetrics(DateTime.Now.ToString("HH:mm:ss"), UserHandler.ConnectedIds.Count);
        }
    }
}