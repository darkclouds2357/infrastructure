using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.MessageBus.RabbitMQ.Connection
{
    public class RabbitMQConfig
    {
        public ConnectionConfig Connection { get; set; }
        public IReadOnlyDictionary<string, ChannelConfig> DispatcherChannels { get; set; }
        public IReadOnlyDictionary<string, ChannelConfig> ListenerChannels { get; set; }
    }
    public class ConnectionConfig
    {
        public string Url { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? RetryCount { get; set; }
        public string VirtualHost { get; set; }
        public int? Port { get; set; }
    }
}
