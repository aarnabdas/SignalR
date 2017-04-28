namespace Microsoft.AspNet.SignalR.Crank
{
    public class MessageItem
    {
        public string Name { get; set; }
        public string Message { get; set; }

        public MessageItem()
        {
            Message = RandomGenerator.Phrase();
        }
    }
}
