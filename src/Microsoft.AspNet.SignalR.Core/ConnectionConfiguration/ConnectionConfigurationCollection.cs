using System.Configuration;

namespace Microsoft.AspNet.SignalR.Core.ConnectionConfiguration
{
    public class ConnectionConfigurationCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets a <see cref="ConnectionConfigurationElement"/> at a given index.
        /// </summary>
        public ConnectionConfigurationElement this[int index]
        {
            get { return (ConnectionConfigurationElement)BaseGet(index); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ConnectionConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConnectionConfigurationElement)element).Name;
        }
    }
}
