using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace Microsoft.AspNet.SignalR.Crank
{
    [System.ComponentModel.Composition.ExportAttribute]
    public class HubConnectionFatory : IConnectionFactory
    {
        Connection IConnectionFactory.CreateConnection(string url)
        {
            return new HubConnectionD(url);
        }

        Connection IConnectionFactory.CreateConnection(string url, string proxy, string channel, string connName)
        {
            return new HubConnectionD(url, proxy, channel, connName);
        }
    }
}
