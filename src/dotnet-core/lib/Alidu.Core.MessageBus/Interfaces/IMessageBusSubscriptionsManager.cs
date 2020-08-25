using Alidu.Core.Domain;
using Alidu.Core.MessageBus.Interfaces;
using System;
using System.Collections.Generic;

namespace Alidu.Core.MessageBus
{
    public interface IMessageBusSubscriptionsManager
    {
        bool IsEmpty { get; }

        event EventHandler<string> OnHandlerRemoved;

        void AddDynamicSubscription<TH>(string messageName)
           where TH : IDynamicMessageHandler;

        void AddSubscription<T, TH>()
           where T : IntegrationMessage
           where TH : IMessageHandler<T>;

        void RemoveSubscription<T, TH>()
             where TH : IMessageHandler<T>
             where T : IntegrationMessage;

        void RemoveDynamicSubscription<TH>(string messageName)
            where TH : IDynamicMessageHandler;

        bool HasSubscriptionsForMessage<T>() where T : IntegrationMessage;

        bool HasSubscriptionsForMessage(string messageName);

        Type GetMessageTypeByName(string messageName);

        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForMessage<T>() where T : IntegrationMessage;

        IEnumerable<SubscriptionInfo> GetHandlersForMessage(string messageName);

        string GetMessageKey<T>();
    }
}
