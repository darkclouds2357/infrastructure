using Alidu.Common.Constants;
using Alidu.Common.Interfaces;
using Alidu.Core.Domain;
using Alidu.MessageBus.Abstractions;
using Alidu.MessageBus.Interfaces;
using Alidu.MessageBus.RabbitMQ.Abstractions;
using Alidu.MessageBus.RabbitMQ.Connection;
using Alidu.MessageBus.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alidu.MessageBus.RabbitMQ
{
    internal class RabbitMQMessageBusSubscribe : RabbitMQMessageBus, IMessageBusSubscribe
    {
        private readonly IMessageBusSubscriptionsManager _subsManager;
        private IModel _consumerChannel;

        public RabbitMQMessageBusSubscribe(IRabbitMQPersistentConnection connection, ILogger<RabbitMQMessageBus> logger, IServiceProvider serviceProvider, IMessageBusSubscriptionsManager subsManager, ChannelConfig channelConfig, int retryCount) : base(connection, logger, serviceProvider, channelConfig, retryCount)
        {
            _subsManager = subsManager ?? throw new ArgumentNullException(nameof(subsManager));
            _consumerChannel = CreateConsumerChannel();
            _subsManager.OnHandlerRemoved += SubsManager_OnHandlerRemoved;
        }

        private void SubsManager_OnHandlerRemoved(object sender, string eventName)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using var channel = _connection.CreateModel();
            channel.QueueUnbind(queue: _channelConfig.QueueName,
                exchange: _channelConfig.Exchange,
                routingKey: _channelConfig.RoutingKey);

            if (_subsManager.IsEmpty)
            {
                _channelConfig.QueueName = string.Empty;
                _consumerChannel.Close();
            }
        }

        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _connection.CreateModel();
            if (!string.IsNullOrWhiteSpace(_channelConfig.Exchange))
            {
                var arguments = !_channelConfig.ExchangeArgs.Any() ? null : _channelConfig.ExchangeArgs;
                channel.ExchangeDeclare(exchange: _channelConfig.Exchange, type: _channelConfig.ExchangeType, durable: true, arguments: arguments);
            }

            var queueArgs = !_channelConfig.QueueArgs.Any() ? null : _channelConfig.QueueArgs;

            if (_channelConfig.IsConfigDeadLetter)
            {
                queueArgs ??= new Dictionary<string, object>();
                queueArgs["x-dead-letter-exchange"] = _channelConfig.DeadLetterExchange;
                queueArgs["x-dead-letter-routing-key"] = _channelConfig.DeadLetterKey;

                CreateDeadLetterChannel();
            }

            channel.QueueDeclare(queue: _channelConfig.QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: queueArgs);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private void CreateDeadLetterChannel()
        {
            var channel = _connection.CreateModel();

            channel.ExchangeDeclare(exchange: _channelConfig.DeadLetterExchange,
                                    type: ExchangeType.Direct,
                                    durable: true);

            channel.QueueDeclare(queue: _channelConfig.DeadLetterKey,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false);
            channel.QueueBind(queue: _channelConfig.DeadLetterKey, exchange: _channelConfig.DeadLetterExchange, routingKey: _channelConfig.DeadLetterKey);
        }

        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.Received += Consumer_Received;

                _consumerChannel.BasicConsume(
                    queue: _channelConfig.QueueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var exchange = eventArgs.Exchange;
            var routing = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var messageNames = GetMessageNamesFromExchange(exchange, routing, _channelConfig.QueueName);
            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }
                var props = eventArgs.BasicProperties;
                if (messageNames.Any())
                {
                    await ProcessMessagesAsync(messageNames, message, props);
                    _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                }
                else
                {
                    _consumerChannel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
                }
            }
            catch (Exception ex)
            {
                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX).
                // For more information see: https://www.rabbitmq.com/dlx.html
                _logger.LogError(ex, "----- ERROR Processing message \"{Message}\"", message);
                _consumerChannel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
        }

        private async Task ProcessMessagesAsync(string[] messageNames, string message, IBasicProperties properties)
        {
            var tasks = new List<Task>();
            foreach (var messageName in messageNames)
            {
                tasks.Add(ProcessMessageAsync(messageName, message, properties));
            }
            await Task.WhenAll(tasks);
        }

        private async Task ProcessMessageAsync(string messageName, string message, IBasicProperties properties)
        {
            _logger.LogTrace("Processing RabbitMQ message name: {MessageName}", messageName);

            if (_subsManager.HasSubscriptionsForMessage(messageName))
            {
                using var scope = _serviceProvider.CreateScope();
                var subscriptions = _subsManager.GetHandlersForMessage(messageName);
                var requestHeader = scope.ServiceProvider.GetRequiredService<IRequestHeader>();
                if (properties.Headers.ContainsKey(TraefikDefault.Claims))
                {
                    var claimsBytes = properties.Headers[TraefikDefault.Claims] as byte[];
                    var claims = Encoding.UTF8.GetString(claimsBytes);
                    requestHeader.SetCredential(claims);
                }

                if (properties.Headers.ContainsKey(TraefikDefault.Channel))
                {
                    var channelBytes = properties.Headers[TraefikDefault.Channel] as byte[];
                    var channel = Encoding.UTF8.GetString(channelBytes);
                    requestHeader.SetChannel(channel);
                }
                if (properties.Headers.ContainsKey(TraefikDefault.CommandId))
                {
                    var commandBytes = properties.Headers[TraefikDefault.CommandId] as byte[];
                    var command = Encoding.UTF8.GetString(commandBytes);
                    requestHeader.SetCommand(command);
                }

                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        if (!(scope.ServiceProvider.GetService(subscription.HandlerType) is IDynamicMessageHandler handler)) continue;

                        dynamic eventData;
                        if (message.StartsWith("{"))
                            eventData = JObject.Parse(message);
                        else
                            eventData = JArray.Parse(message);

                        await Task.Yield();
                        await handler.Handle(properties.CorrelationId, eventData);
                    }
                    else
                    {
                        var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                        if (handler == null) continue;
                        var eventType = _subsManager.GetMessageTypeByName(messageName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IMessageHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { properties.CorrelationId, integrationEvent });
                    }
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", messageName);
            }
        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }

            _subsManager.Clear();
        }

        public void Subscribe<T, TH>()
            where T : BaseMessage
            where TH : IMessageHandler<T>
        {
            var messageName = _subsManager.GetMessageKey<T>();
            DoInternalSubscription();

            _logger.LogInformation("Subscribing to event {MessageName} with {MessageHandler}", messageName, typeof(TH).GetGenericTypeName());

            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume();
        }

        public void SubscribeDynamic<TH>(string messageName) where TH : IDynamicMessageHandler
        {
            _logger.LogInformation("Subscribing to dynamic event {MessageName} with {MessageHandler}", messageName, typeof(TH).GetGenericTypeName());

            DoInternalSubscription();
            _subsManager.AddDynamicSubscription<TH>(messageName);
            StartBasicConsume();
        }

        public void Unsubscribe<T, TH>()
            where T : BaseMessage
            where TH : IMessageHandler<T>
        {
            var messageName = _subsManager.GetMessageKey<T>();

            _logger.LogInformation("Unsubscribing from message name {MessageName}", messageName);

            _subsManager.RemoveSubscription<T, TH>();
        }

        public void UnsubscribeDynamic<TH>(string messageName) where TH : IDynamicMessageHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(messageName);
        }

        private void DoInternalSubscription()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using var channel = _connection.CreateModel();
            channel.QueueBind(queue: _channelConfig.QueueName,
                              exchange: _channelConfig.Exchange,
                              routingKey: _channelConfig.RoutingKey);
        }

        public string[] GetMessageNamesFromExchange(string exchange, string routing, string queueName)
        {
            using var scope = _serviceProvider.CreateScope();
            var rabbitMQConfig = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMQConfig>>().Value;
            var messageBusConfig = scope.ServiceProvider.GetRequiredService<IOptions<MessageBusConfig>>().Value;

            var channels = rabbitMQConfig.ListenerChannels.Where(c => c.Value.Exchange == exchange && c.Value.IsRoutingMatch(routing));
            if (!string.IsNullOrWhiteSpace(queueName))
                channels = channels.Where(c => c.Value.QueueName == queueName);
            var channelKeys = channels.Select(c => c.Key).Distinct();

            return messageBusConfig.ListenedMessageRoutes.Where(r => r.Value.Channels.Any(c => channelKeys.Contains(c))).Select(r => r.Key).Distinct().ToArray();
        }
    }
}