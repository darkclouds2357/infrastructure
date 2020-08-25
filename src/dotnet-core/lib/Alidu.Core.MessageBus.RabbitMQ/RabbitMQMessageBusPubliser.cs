using Alidu.Core.Common.Constants;
using Alidu.Core.Common.Interfaces;
using Alidu.Core.Domain;
using Alidu.Core.MessageBus.Interfaces;
using Alidu.Core.MessageBus.RabbitMQ.Abstractions;
using Alidu.Core.MessageBus.RabbitMQ.Connection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Alidu.Core.MessageBus.RabbitMQ
{
    internal class RabbitMQMessageBusPubliser : RabbitMQMessageBus, IMessageBusPubliser
    {
        public RabbitMQMessageBusPubliser(IRabbitMQPersistentConnection connection, ILogger<RabbitMQMessageBus> logger, IServiceProvider serviceProvider, ChannelConfig channelConfig, int retryCount) : base(connection, logger, serviceProvider, channelConfig, retryCount)
        {
        }
        private IBasicProperties GetBasicProperties(IModel model, out string correlationId)
        {
            using var scope = _serviceProvider.CreateScope();
            var requestCredential = scope.ServiceProvider.GetRequiredService<IRequestCredential>();
            var requestTransaction = scope.ServiceProvider.GetRequiredService<IRequestCommand>();
            var requestChannel = scope.ServiceProvider.GetService<IRequestChannel>();
            var properties = model.CreateBasicProperties();
            if (properties.Headers == null)
                properties.Headers = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(requestCredential.ToString()))
                properties.Headers[TraefikDefault.Claims] = Encoding.UTF8.GetBytes(requestCredential.ToString());

            if (!string.IsNullOrWhiteSpace(requestChannel.ToString()))
                properties.Headers[TraefikDefault.Channel] = Encoding.UTF8.GetBytes(requestChannel?.ToString());

            if (!string.IsNullOrWhiteSpace(requestTransaction.CommandId))
                properties.Headers[TraefikDefault.CommandId] = Encoding.UTF8.GetBytes(requestTransaction.CommandId);

            foreach (var item in _channelConfig.Headers)
            {
                properties.Headers[item.Key] = Encoding.UTF8.GetBytes(item.Value);
            }
            correlationId = requestTransaction.CommandId ?? Guid.NewGuid().ToString();

            properties.DeliveryMode = 2; // persistent
            properties.CorrelationId = correlationId.ToString();

            return properties;
        }

        public string Publish(IntegrationMessage messagePayload)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }
            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish message: {MessageId} after {Timeout}s ({ExceptionMessage})", messagePayload.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            _logger.LogTrace("Creating RabbitMQ channel to publish message: {MessageId} ({MessageName})", messagePayload.Id, messagePayload.MessageName);

            using var channel = _connection.CreateModel();
            _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {MessageId}", messagePayload.Id);

            if (!string.IsNullOrWhiteSpace(_channelConfig.Exchange))
            {
                var arguments = !_channelConfig.ExchangeArgs.Any() ? null : _channelConfig.ExchangeArgs;
                channel.ExchangeDeclare(exchange: _channelConfig.Exchange, type: _channelConfig.ExchangeType, durable: true, arguments: arguments);
            }
            if (!string.IsNullOrWhiteSpace(_channelConfig.QueueName))
                channel.QueueBind(queue: _channelConfig.QueueName, exchange: _channelConfig.Exchange, routingKey: _channelConfig.RoutingKey);

            var properties = GetBasicProperties(channel, out string correlationId);

            var message = JsonConvert.SerializeObject(messagePayload, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                _logger.LogTrace("Publishing message to RabbitMQ: {MessageId}", messagePayload.Id);

                channel.BasicPublish(
                    exchange: _channelConfig.Exchange,
                    routingKey: _channelConfig.RoutingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });

            return correlationId;
        }

        public string PublishDynamic(dynamic messagePayload)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }
            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish dynamic message after {Timeout}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                });

            _logger.LogTrace("Creating RabbitMQ channel to publish dynamic message.");

            using var channel = _connection.CreateModel();
            _logger.LogTrace("Declaring RabbitMQ exchange to publish dynamic message");

            if (!string.IsNullOrWhiteSpace(_channelConfig.Exchange))
            {
                var arguments = !_channelConfig.ExchangeArgs.Any() ? null : _channelConfig.ExchangeArgs;
                channel.ExchangeDeclare(exchange: _channelConfig.Exchange, type: _channelConfig.ExchangeType, durable: true, arguments: arguments);
            }
            if (!string.IsNullOrWhiteSpace(_channelConfig.QueueName))
                channel.QueueBind(queue: _channelConfig.QueueName, exchange: _channelConfig.Exchange, routingKey: _channelConfig.RoutingKey);

            var properties = GetBasicProperties(channel, out string correlationId);
            messagePayload.SetTransactionId(correlationId);

            var message = JsonConvert.SerializeObject(messagePayload, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                _logger.LogTrace($"Publishing dynamic message to RabbitMQ  {message}");


                channel.BasicPublish(
                    exchange: _channelConfig.Exchange,
                    routingKey: _channelConfig.RoutingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });

            return correlationId;
        }
    }
}
