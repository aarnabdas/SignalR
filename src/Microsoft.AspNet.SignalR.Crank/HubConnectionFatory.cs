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
    }
}
