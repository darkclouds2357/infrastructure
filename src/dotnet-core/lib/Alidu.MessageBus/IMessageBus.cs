using Alidu.MessageBus.Abstractions;
using Alidu.MessageBus.Interfaces;

namespace Alidu.MessageBus
{
    public interface IMessageBus
    {
        void Publish(string messageName, dynamic message);

        void Publish<T>(T message) where T : BaseMessage;

        void Subscribe<T, TH>()
            where T : BaseMessage
            where TH : IMessageHandler<T>;

        void SubscribeDynamic<TH>(string messageName)
            where TH : IDynamicMessageHandler;

        void UnsubscribeDynamic<TH>(string messageName)
            where TH : IDynamicMessageHandler;

        void Unsubscribe<T, TH>()
            where TH : IMessageHandler<T>
            where T : BaseMessage;
    }
}