using Alidu.MessageBus.Abstractions;
using Alidu.MessageBus.Interfaces;
using Alidu.MessageBus.RabbitMQ.Abstractions;
using Alidu.MessageBus.RabbitMQ.Connection;
using Alidu.MessageBus.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.MessageBus.RabbitMQ
{
    public class RabbitMQMessageBusManager : MessageBusManager
    {
        public RabbitMQMessageBusManager(IRabbitMQPersistentConnection connection, ILogger<RabbitMQMessageBus> logger, IServiceProvider serviceProvider, IMessageBusSubscriptionsManager subsManager, IOptions<MessageBusConfig> setting, MessageTypeConfig messageTypesConfig, RabbitMQConfig rabbitMQConfig, int retryCount = 5)
        {
            _messageTypes = messageTypesConfig.MessageTypes;
            var messageBusConfig = setting?.Value ?? throw new ArgumentNullException(nameof(setting));

            var dispatchedMessageBus = new Dictionary<string, IMessageBusPubliser>();
            if (rabbitMQConfig.DispatcherChannels != null)
            {
                foreach (var item in rabbitMQConfig.DispatcherChannels)
                {
                    dispatchedMessageBus[item.Key] = new RabbitMQMessageBusPubliser(connection, logger, serviceProvider, item.Value, retryCount);
                }
            }

            _messageBusPubliser = dispatchedMessageBus;
            _dispatchedMessageRoutes = messageBusConfig.DispatchedMessageRoutes ?? new Dictionary<string, MessageRouteConfig>();

            var listenedMessageBus = new Dictionary<string, IMessageBusSubscribe>();
            if (rabbitMQConfig.ListenerChannels != null)
            {
                foreach (var item in rabbitMQConfig.ListenerChannels)
                {
                    listenedMessageBus[item.Key] = new RabbitMQMessageBusSubscribe(connection, logger, serviceProvider, subsManager, item.Value, retryCount);
                }
            }
            _messageBusSubscribe = listenedMessageBus;
            _listenedMessageRoutes = messageBusConfig.ListenedMessageRoutes ?? new Dictionary<string, MessageRouteConfig>();
        }

    }
}
