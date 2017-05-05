using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRSample
{
    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }
}