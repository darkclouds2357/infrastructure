using Alidu.MessageBus.Abstractions;
using Alidu.MessageBus.Interfaces;
using Alidu.MessageBus.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alidu.MessageBus.InMemory
{
    public class InMemoryMessageBusManager : MessageBusManager
    {
        public InMemoryMessageBusManager(IServiceProvider serviceProvider,
                                           IMessageBusSubscriptionsManager subsManager,
                                           IOptions<MessageBusConfig> messageBusConfigOption,
                                           MessageTypeConfig messageTypesConfig,
                                           InMemoryQueue inMemoryQueue)
        {
            _messageTypes = messageTypesConfig.MessageTypes;
            var messageBusConfig = messageBusConfigOption?.Value ?? throw new ArgumentNullException(nameof(messageBusConfigOption));

            var dispatchedMessageBus = new Dictionary<string, IMessageBusPubliser>();
            if (messageBusConfig.DispatchedMessageRoutes != null)
            {
                var channels = messageBusConfig.DispatchedMessageRoutes.Select(c => c.Value);
                foreach (var item in channels)
                {
                    foreach (var channel in item.Channels)
                    {
                        dispatchedMessageBus[channel] = new InMemoryMessageBusPubliser(serviceProvider, channel, inMemoryQueue);
                    }
                }
            }

            _messageBusPubliser = dispatchedMessageBus;
            _dispatchedMessageRoutes = messageBusConfig.DispatchedMessageRoutes ?? new Dictionary<string, MessageRouteConfig>();

            var listenedMessageBus = new Dictionary<string, IMessageBusSubscribe>();
            if (messageBusConfig.ListenedMessageRoutes != null)
            {
                var routes = messageBusConfig.ListenedMessageRoutes.Select(c => c.Value);
                foreach (var route in routes)
                {
                    foreach (var channel in route.Channels)
                    {
                        listenedMessageBus[channel] = new InMemoryMessageBusSubscribe(serviceProvider, subsManager, inMemoryQueue);
                    }
                }
            }
            _messageBusSubscribe = listenedMessageBus;
            _listenedMessageRoutes = messageBusConfig.ListenedMessageRoutes ?? new Dictionary<string, MessageRouteConfig>();
        }
    }
}