using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

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
