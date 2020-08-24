using Alidu.Core.MessageBus.RabbitMQ.Connection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.MessageBus.RabbitMQ.Abstractions
{
    public abstract class RabbitMQMessageBus
    {
        protected readonly IRabbitMQPersistentConnection _connection;
        protected readonly ILogger<RabbitMQMessageBus> _logger;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ChannelConfig _channelConfig;
        protected readonly int _retryCount;

        protected RabbitMQMessageBus(IRabbitMQPersistentConnection connection, ILogger<RabbitMQMessageBus> logger, IServiceProvider serviceProvider, ChannelConfig channelConfig, int retryCount)
        {
            _connection = connection;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _channelConfig = channelConfig;
            _retryCount = retryCount;
        }
    }
}
