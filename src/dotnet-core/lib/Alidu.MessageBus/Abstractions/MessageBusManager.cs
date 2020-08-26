using Alidu.Core.Domain;
using Alidu.Core.Domain.Interfaces;
using Alidu.CQRS;
using Alidu.MessageBus.Interfaces;
using Alidu.MessageBus.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alidu.MessageBus.Abstractions
{
    public abstract class MessageBusManager : IMessageBus, IDisposable
    {
        protected IReadOnlyDictionary<string, IMessageBusPubliser> _messageBusPubliser;
        protected IReadOnlyDictionary<string, MessageRouteConfig> _dispatchedMessageRoutes;

        protected IReadOnlyDictionary<string, IMessageBusSubscribe> _messageBusSubscribe;
        protected IReadOnlyDictionary<string, MessageRouteConfig> _listenedMessageRoutes;
        protected IReadOnlyDictionary<Type, string> _messageTypes;


        private IEnumerable<IMessageBusPubliser> GetMessageBusPubliser(string eventName)
        {
            var channels = _dispatchedMessageRoutes.Where(v => v.Key == eventName).SelectMany(v => v.Value.Channels).Distinct();
            foreach (var channel in channels)
            {
                yield return _messageBusPubliser[channel];
            }
        }

        private IEnumerable<IMessageBusPubliser> GetEventBusPubliser<T>()
        {
            if (_messageTypes.ContainsKey(typeof(T)))
                return GetMessageBusPubliser(_messageTypes[typeof(T)]);
            throw new ArgumentNullException(typeof(T).Name);
        }

        private bool ValidateDispatchMessageType(string messageName, object message)
        {
            if (!_dispatchedMessageRoutes.ContainsKey(messageName))
                throw new ArgumentNullException(messageName);
            var routeConfig = _dispatchedMessageRoutes[messageName];
            if (!(routeConfig.PayloadSchema?.Any() ?? false))
                return true;

            return true;
        }

        private bool ValidateDispatchMessageType<T>(T message)
        {
            if (_messageTypes.ContainsKey(typeof(T)))
                return ValidateDispatchMessageType(_messageTypes[typeof(T)], message);
            throw new ArgumentNullException(typeof(T).Name);
        }

        public void Publish(string messageName, dynamic message)
        {
            if (!ValidateDispatchMessageType(messageName, message))
                throw new ArgumentException();
            var eventBus = GetMessageBusPubliser(messageName);
            foreach (var item in eventBus)
            {
                item.PublishDynamic(message);
            }
        }

        public void Publish<T>(T message) where T : BaseMessage
        {
            if (!ValidateDispatchMessageType<T>(message))
                throw new ArgumentException();
            var eventBus = GetEventBusPubliser<T>();
            foreach (var item in eventBus)
            {
                item.Publish(message);
            }
        }

        private IMessageBusSubscribe[] GetMessageBusConsumer(string messageName)
        {
            if (!_listenedMessageRoutes.ContainsKey(messageName))
                return Enumerable.Empty<IMessageBusSubscribe>().ToArray(); //throw new ArgumentException();
            var channels = _listenedMessageRoutes[messageName].Channels;
            return _messageBusSubscribe.Where(c => channels.Contains(c.Key)).Select(c => c.Value).ToArray();
        }

        private IMessageBusSubscribe[] GetEventBusConsumer<T>()
        {
            if (_messageTypes.ContainsKey(typeof(T)))
                return GetMessageBusConsumer(_messageTypes[typeof(T)]);
            return Enumerable.Empty<IMessageBusSubscribe>().ToArray(); //throw new ArgumentNullException(typeof(T).Name);
        }

        public void Subscribe<T, TH>()
            where T : BaseMessage
            where TH : IMessageHandler<T>
        {
            var consumers = GetEventBusConsumer<T>();
            foreach (var consumer in consumers)
            {
                consumer.Subscribe<T, TH>();
            }
        }

        public void SubscribeDynamic<TH>(string messageName) where TH : IDynamicMessageHandler
        {
            var consumers = GetMessageBusConsumer(messageName);
            foreach (var consumer in consumers)
            {
                consumer.SubscribeDynamic<TH>(messageName);
            }
        }

        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicMessageHandler
        {
            var consumers = GetMessageBusConsumer(eventName);
            foreach (var consumer in consumers)
            {
                consumer.UnsubscribeDynamic<TH>(eventName);
            }
        }

        public void Unsubscribe<T, TH>()
            where T : BaseMessage
            where TH : IMessageHandler<T>
        {
            var consumers = GetEventBusConsumer<T>();
            foreach (var consumer in consumers)
            {
                consumer.Unsubscribe<T, TH>();
            }
        }

        public virtual void Dispose()
        {
            foreach (var item in _messageBusSubscribe)
            {
                item.Value.Dispose();
            }
        }
    }
}
