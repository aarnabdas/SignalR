using System.Configuration;

namespace Microsoft.AspNet.SignalR.Core.ConnectionConfiguration
{
    public class ConnectionConfigurationSection : ConfigurationSection
    {
        private const string ConnectionCollectionName = "connections";

        private const string ConnectionElementName = "connection";

        /// <summary>
        /// Connections specified
        /// </summary>
        [ConfigurationProperty(ConnectionCollectionName, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ConnectionConfigurationCollection), AddItemName = ConnectionElementName)]
        public ConnectionConfigurationCollection Connections
        {
            get { return (ConnectionConfigurationCollection)this[ConnectionCollectionName]; }
            set { this[ConnectionCollectionName] = value; }
        }
    }
}
