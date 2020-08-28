using Alidu.Common.Interfaces;
using Alidu.MessageBus.Abstractions;
using Alidu.MessageBus.Interfaces;
using Alidu.MessageBus.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alidu.MessageBus.InMemory
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
            where T : BaseMessage
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
            where T : BaseMessage
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


            if (queueMessage.Header != null)
            {
                var header = queueMessage.Header;
                var requestHeader = scope.ServiceProvider.GetRequiredService<IRequestHeader>();
                requestHeader.SetCredential(header.Credential.ToString());
                requestHeader.SetChannel(header.Channel.ToString());
                requestHeader.SetCommand(header.Command.ToString());
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