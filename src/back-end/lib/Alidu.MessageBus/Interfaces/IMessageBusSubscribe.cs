using Alidu.MessageBus.Abstractions;
using System;

namespace Alidu.MessageBus.Interfaces
{
    public interface IMessageBusSubscribe : IDisposable
    {
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