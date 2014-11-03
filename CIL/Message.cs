namespace WpfMdiApp1.CIL
{
    public enum MessageType { Loaded }
    
    public class Message
    {
        public MessageType MessageType { get; private set; }

        public object MessageBody { get; private set; }

        public Message(MessageType type, object body)
        {
            MessageType = type;
            MessageBody = body;
        }

        public Message(MessageType type)
        {
            MessageType = type;
        }
    }
}
