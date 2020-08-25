using Alidu.Core.Domain;
using Alidu.Core.MessageBus.Interfaces;

namespace Alidu.Core.MessageBus
{
    public interface IMessageBus
    {
        void Publish(string messageName, dynamic message);

        void Publish<T>(T message) where T : IntegrationMessage;

        void Subscribe<T, TH>()
            where T : IntegrationMessage
            where TH : IMessageHandler<T>;

        void SubscribeDynamic<TH>(string messageName)
            where TH : IDynamicMessageHandler;

        void UnsubscribeDynamic<TH>(string messageName)
            where TH : IDynamicMessageHandler;

        void Unsubscribe<T, TH>()
            where TH : IMessageHandler<T>
            where T : IntegrationMessage;
    }
}
