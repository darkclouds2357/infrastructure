using Alidu.Core.MessageBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.MessageBus
{
    public interface IMessageBusSubscriptionsManager
    {
        bool IsEmpty { get; }

        event EventHandler<string> OnHandlerRemoved;

        void AddDynamicSubscription<TH>(string messageName)
           where TH : IDynamicMessageHandler;

        void AddSubscription<T, TH>()
           where T : MessagePayload
           where TH : IMessageHandler<T>;

        void RemoveSubscription<T, TH>()
             where TH : IMessageHandler<T>
             where T : MessagePayload;

        void RemoveDynamicSubscription<TH>(string messageName)
            where TH : IDynamicMessageHandler;

        bool HasSubscriptionsForMessage<T>() where T : MessagePayload;

        bool HasSubscriptionsForMessage(string messageName);

        Type GetMessageTypeByName(string messageName);

        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForMessage<T>() where T : MessagePayload;

        IEnumerable<SubscriptionInfo> GetHandlersForMessage(string messageName);

        string GetMessageKey<T>();
    }
}
