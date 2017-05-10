using System.Configuration;

namespace Microsoft.AspNet.SignalR.Core.ConnectionConfiguration
{
    /// <summary>
    /// Connection configuration for each of the connections
    /// </summary>
    public class ConnectionConfigurationElement : ConfigurationElement
    {
        private const string NameAttribute = "name";

        private const string UrlAttribute = "url";

        private const string ProxyAttribute = "proxy";

        private const string ChannelAttribute = "channel";

        private const string ConnectionsCountAttribute = "connections";

        /// <summary>
        /// The url to be used for the connections
        /// </summary>
        [ConfigurationProperty(NameAttribute, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this[NameAttribute];
            }
            set
            {
                this[NameAttribute] = value;
            }
        }

        /// <summary>
        /// The url to be used for the connections
        /// </summary>
        [ConfigurationProperty(UrlAttribute, IsRequired = true)]
        public string Url
        {
            get
            {
                return (string)this[UrlAttribute];
            }
            set
            {
                this[UrlAttribute] = value;
            }
        }

        /// <summary>
        /// The proxy or the hub name
        /// </summary>
        [ConfigurationProperty(ProxyAttribute, IsRequired = true)]
        public string Proxy
        {
            get
            {
                return (string)this[ProxyAttribute];
            }
            set
            {
                this[ProxyAttribute] = value;
            }
        }

        /// <summary>
        /// Channel to subscribe to
        /// </summary>
        [ConfigurationProperty(ChannelAttribute, IsRequired = false)]
        public string Channel
        {
            get
            {
                return (string)this[ChannelAttribute];
            }
            set
            {
                this[ChannelAttribute] = value;
            }
        }

        /// <summary>
        /// Total number of connections to make
        /// </summary>
        [ConfigurationProperty(ConnectionsCountAttribute, IsRequired = true)]
        public string ConnectionsCount
        {
            get
            {
                return (string)this[ConnectionsCountAttribute];
            }
            set
            {
                this[ConnectionsCountAttribute] = value;
            }
        }

        public int Connections
        {
            get 
            {                
                int connections = 0;
                int.TryParse(this.ConnectionsCount, out connections);
                return connections;
            }
        }
    }
}
