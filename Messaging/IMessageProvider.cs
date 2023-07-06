namespace Netcode.io.Messaging
{
    internal interface IMessageProvider
    {
        List<MessagingSystem.MessageWithHandler> GetMessages();
    }
}
