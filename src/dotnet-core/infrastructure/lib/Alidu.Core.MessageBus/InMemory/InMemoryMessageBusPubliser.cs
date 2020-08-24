using Alidu.Core.Common.Interfaces;
using Alidu.Core.MessageBus.Abstractions;
using Alidu.Core.MessageBus.Interfaces;
using Alidu.Core.MessageBus.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.MessageBus.InMemory
{
    public class InMemoryMessageBusPubliser : IMessageBusPubliser
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _channel;
        private readonly InMemoryQueue _inMemoryQueue;

        public InMemoryMessageBusPubliser(IServiceProvider serviceProvider, string channel, InMemoryQueue inMemoryQueue)
        {
            _serviceProvider = serviceProvider;
            _channel = channel;
            _inMemoryQueue = inMemoryQueue;
        }

        public string Publish(MessagePayload message)
        {
            return PublishDynamic(message);
        }

        public string PublishDynamic(dynamic message)
        {
            using var scope = _serviceProvider.CreateScope();

            var requestCredential = scope.ServiceProvider.GetRequiredService<IRequestCredential>();
            var requestTransaction = scope.ServiceProvider.GetRequiredService<IRequestTransaction>();
            var requestChannel = scope.ServiceProvider.GetService<IRequestChannel>();

            var queueMessage = new QueueMessage(message, requestCredential, requestChannel, requestTransaction);

            _inMemoryQueue.Publish(this, _channel, queueMessage);

            return queueMessage.CommandId;
        }
    }
}
