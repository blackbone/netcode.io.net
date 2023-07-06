using Netcode.io.Serialization;

namespace Netcode.io.Messaging
{
    internal interface IMessageSender
    {
        void Send(ulong clientId, NetworkDelivery delivery, FastBufferWriter batchData);
    }
}
