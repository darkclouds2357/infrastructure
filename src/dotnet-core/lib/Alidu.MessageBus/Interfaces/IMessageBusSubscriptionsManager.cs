using Alidu.CQRS;
using Alidu.MessageBus.Interfaces;
using System;
using System.Collections.Generic;

namespace Alidu.MessageBus
{
    public interface IMessageBusSubscriptionsManager
    {
        bool IsEmpty { get; }

        event EventHandler<string> OnHandlerRemoved;

        void AddDynamicSubscription<TH>(string messageName)
           where TH : IDynamicMessageHandler;

        void AddSubscription<T, TH>()
           where T : BaseMessage
           where TH : IMessageHandler<T>;

        void RemoveSubscription<T, TH>()
             where TH : IMessageHandler<T>
             where T : BaseMessage;

        void RemoveDynamicSubscription<TH>(string messageName)
            where TH : IDynamicMessageHandler;

        bool HasSubscriptionsForMessage<T>() where T : BaseMessage;

        bool HasSubscriptionsForMessage(string messageName);

        Type GetMessageTypeByName(string messageName);

        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForMessage<T>() where T : BaseMessage;

        IEnumerable<SubscriptionInfo> GetHandlersForMessage(string messageName);

        string GetMessageKey<T>();
    }
}