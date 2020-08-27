using Alidu.MessageBus.Abstractions;
using Alidu.MessageBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alidu.MessageBus.InMemory
{
    internal class InMemoryMessageBusSubscriptionsManager : IMessageBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly IReadOnlyDictionary<Type, string> _messageTypes;

        public event EventHandler<string> OnHandlerRemoved;

        public InMemoryMessageBusSubscriptionsManager(IReadOnlyDictionary<Type, string> messageTypes)
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _messageTypes = messageTypes ?? new Dictionary<Type, string>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();

        public void Clear() => _handlers.Clear();

        public void AddDynamicSubscription<TH>(string messageName)
            where TH : IDynamicMessageHandler
        {
            DoAddSubscription(typeof(TH), messageName, isDynamic: true);
        }

        public void AddSubscription<T, TH>()
            where T : BaseMessage
            where TH : IMessageHandler<T>
        {
            var messageName = GetMessageKey<T>();

            DoAddSubscription(typeof(TH), messageName, isDynamic: false);
        }

        private void DoAddSubscription(Type handlerType, string messageName, bool isDynamic)
        {
            if (!HasSubscriptionsForMessage(messageName))
            {
                _handlers.Add(messageName, new List<SubscriptionInfo>());
            }

            if (!_handlers[messageName].Any(s => s.HandlerType == handlerType))
            {
                if (isDynamic)
                {
                    _handlers[messageName].Add(SubscriptionInfo.Dynamic(handlerType));
                }
                else
                {
                    _handlers[messageName].Add(SubscriptionInfo.Typed(handlerType));
                }
            }
        }

        public void RemoveDynamicSubscription<TH>(string messageName)
            where TH : IDynamicMessageHandler
        {
            var handlerToRemove = FindDynamicSubscriptionToRemove<TH>(messageName);
            DoRemoveHandler(messageName, handlerToRemove);
        }

        public void RemoveSubscription<T, TH>()
            where TH : IMessageHandler<T>
            where T : BaseMessage
        {
            var handlerToRemove = FindSubscriptionToRemove<T, TH>();
            var messageName = GetMessageKey<T>();
            DoRemoveHandler(messageName, handlerToRemove);
        }

        private void DoRemoveHandler(string messageName, SubscriptionInfo subsToRemove)
        {
            if (subsToRemove != null)
            {
                _handlers[messageName].Remove(subsToRemove);
                if (!_handlers[messageName].Any())
                {
                    _handlers.Remove(messageName);
                    RaiseOnHandlerRemoved(messageName);
                }
            }
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForMessage<T>() where T : BaseMessage
        {
            var key = GetMessageKey<T>();
            return GetHandlersForMessage(key);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForMessage(string messageName) => _handlers[messageName];

        private void RaiseOnHandlerRemoved(string messageName)
        {
            var handler = OnHandlerRemoved;
            handler?.Invoke(this, messageName);
        }

        private SubscriptionInfo FindDynamicSubscriptionToRemove<TH>(string messageName)
            where TH : IDynamicMessageHandler
        {
            return DoFindSubscriptionToRemove(messageName, typeof(TH));
        }

        private SubscriptionInfo FindSubscriptionToRemove<T, TH>()
             where T : BaseMessage
             where TH : IMessageHandler<T>
        {
            var messageName = GetMessageKey<T>();
            return DoFindSubscriptionToRemove(messageName, typeof(TH));
        }

        private SubscriptionInfo DoFindSubscriptionToRemove(string messageName, Type handlerType)
        {
            if (!HasSubscriptionsForMessage(messageName))
            {
                return null;
            }

            return _handlers[messageName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        public bool HasSubscriptionsForMessage<T>() where T : BaseMessage
        {
            var key = GetMessageKey<T>();
            return HasSubscriptionsForMessage(key);
        }

        public bool HasSubscriptionsForMessage(string messageName) => _handlers.ContainsKey(messageName);

        public Type GetMessageTypeByName(string messageName) => _messageTypes.FirstOrDefault(t => t.Value == messageName).Key;

        public string GetMessageKey<T>()
        {
            if (_messageTypes.ContainsKey(typeof(T)))
                return _messageTypes[typeof(T)];
            throw new ArgumentNullException(typeof(T).Name);
        }
    }
}