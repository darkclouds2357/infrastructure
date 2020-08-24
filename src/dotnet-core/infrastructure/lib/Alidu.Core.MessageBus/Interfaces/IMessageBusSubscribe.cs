using Alidu.Core.MessageBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.MessageBus.Interfaces
{
    public interface IMessageBusSubscribe : IDisposable
    {
        void Subscribe<T, TH>()
               where T : MessagePayload
               where TH : IMessageHandler<T>;

        void SubscribeDynamic<TH>(string messageName)
            where TH : IDynamicMessageHandler;

        void UnsubscribeDynamic<TH>(string messageName)
            where TH : IDynamicMessageHandler;

        void Unsubscribe<T, TH>()
            where TH : IMessageHandler<T>
            where T : MessagePayload;
    }
}
