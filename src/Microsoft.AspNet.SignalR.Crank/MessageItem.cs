using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.AspNet.SignalR.Crank
{
    [Serializable]
    public class MessageItem
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }

        public MessageItem()
        {
            Guid = System.Guid.NewGuid().ToString();
            Message = RandomGenerator.Phrase();
        }
    }

    [Serializable]
    public class MessageItems
    {
        [XmlElement("MessageItem")]
        public MessageItem[] Messages { get; set; }
    }
}
