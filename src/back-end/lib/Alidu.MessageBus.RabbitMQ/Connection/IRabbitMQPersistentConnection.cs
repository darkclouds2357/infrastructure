using RabbitMQ.Client;
using System;

namespace Alidu.MessageBus.RabbitMQ.Connection
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        IConnectionFactory ConnectionFactory { get; }
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}