using Alidu.Core.Common.Interfaces;
using Alidu.Core.MessageBus.Abstractions;
using Alidu.Core.MessageBus.Interfaces;
using Alidu.Core.MessageBus.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alidu.Core.MessageBus.InMemory
{
    public class InMemoryMessageBusSubscribe : IMessageBusSubscribe
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageBusSubscriptionsManager _subsManager;
        private readonly InMemoryQueue _inMemoryQueue;

        public InMemoryMessageBusSubscribe(IServiceProvider serviceProvider, IMessageBusSubscriptionsManager subsManager, InMemoryQueue inMemoryQueue)
        {
            _serviceProvider = serviceProvider;
            _subsManager = subsManager;
            _inMemoryQueue = inMemoryQueue;
        }

        public void Subscribe<T, TH>()
            where T : MessagePayload
            where TH : IMessageHandler<T>
        {
            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume();
        }

        public void SubscribeDynamic<TH>(string messageName) where TH : IDynamicMessageHandler
        {
            _subsManager.AddDynamicSubscription<TH>(messageName);
            StartBasicConsume();
        }

        public void Unsubscribe<T, TH>()
            where T : MessagePayload
            where TH : IMessageHandler<T>
        {
            _subsManager.RemoveSubscription<T, TH>();
        }

        public void UnsubscribeDynamic<TH>(string messageName) where TH : IDynamicMessageHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(messageName);
        }

        private void StartBasicConsume()
        {
            _inMemoryQueue.MessageReceived += Message_Received;
        }

        private void Message_Received(object sender, string channel)
        {
            var messageNames = GetMessagesFromChannel(channel);
            if (messageNames.Any() && _inMemoryQueue.GetQueues(channel).Any())
            {
                ProcessMessagesAsync(messageNames, channel).GetAwaiter().GetResult();

            }

        }

        private string[] GetMessagesFromChannel(string channel)
        {
            using var scope = _serviceProvider.CreateScope();
            var messageBusConfig = scope.ServiceProvider.GetRequiredService<IOptions<MessageBusConfig>>().Value;

            var messageNames = messageBusConfig.ListenedMessageRoutes.Where(e => e.Value.Channels.Contains(channel)).Select(e => e.Key);
            return messageNames.ToArray();
        }

        private async Task ProcessMessagesAsync(string[] messageNames, string channel)
        {
            var tasks = new List<Task>();
            foreach (var messageName in messageNames)
            {
                tasks.Add(ProcessMessageAsync(messageName, channel));
            }
            await Task.WhenAll(tasks);
        }

        private async Task ProcessMessageAsync(string messageName, string eventChannel)
        {
            if (!_subsManager.HasSubscriptionsForMessage(messageName))
                return;

            var queueMessage = _inMemoryQueue.GetQueues(eventChannel).Dequeue();

            if (queueMessage == null)
                return;

            using var scope = _serviceProvider.CreateScope();

            var subscriptions = _subsManager.GetHandlersForMessage(messageName);
            if (queueMessage.Credential != null)
            {
                var claims = queueMessage.Credential.ToString();
                var requestCredential = scope.ServiceProvider.GetRequiredService<IRequestCredential>();
                requestCredential.SetClaims(claims);
            }

            if (queueMessage.Channel != null)
            {
                var channel = queueMessage.Channel.ToString();
                var requestChannel = scope.ServiceProvider.GetRequiredService<IRequestChannel>();
                requestChannel.SetChannel(channel);
            }
            if (queueMessage.Transaction != null)
            {
                var command = queueMessage.Transaction.CommandId;
                var requestTransaction = scope.ServiceProvider.GetRequiredService<IRequestTransaction>();
                requestTransaction.SetCommandId(command);
            }

            var message = queueMessage.Payload;
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
                    await handler.Handle(queueMessage.CommandId, eventData);
                }
                else
                {
                    var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                    if (handler == null) continue;
                    var eventType = _subsManager.GetMessageTypeByName(messageName);
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                    var concreteType = typeof(IMessageHandler<>).MakeGenericType(eventType);

                    await Task.Yield();
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { queueMessage.CommandId, integrationEvent });
                }
            }

        }

        public void Dispose()
        {
        }
    }
}
