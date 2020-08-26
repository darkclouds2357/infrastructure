using Alidu.Common.Interfaces;
using Alidu.Core.Domain;
using Alidu.Core.Domain.Interfaces;
using Alidu.CQRS;
using Alidu.MessageBus.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Alidu.MessageBus.InMemory
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

        public string Publish(BaseMessage message)
        {
            return PublishDynamic(message);
        }

        public string PublishDynamic(dynamic message)
        {
            using var scope = _serviceProvider.CreateScope();

            var requestCredential = scope.ServiceProvider.GetRequiredService<IRequestCredential>();
            var requestTransaction = scope.ServiceProvider.GetRequiredService<IRequestCommand>();
            var requestChannel = scope.ServiceProvider.GetService<IRequestChannel>();

            var queueMessage = new QueueMessage(message, requestCredential, requestChannel, requestTransaction);

            _inMemoryQueue.Publish(this, _channel, queueMessage);

            return queueMessage.CommandId;
        }
    }
}
