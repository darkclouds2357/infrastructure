using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Alidu.MessageBus.RabbitMQ.Connection
{
    public class ChannelConfig
    {
        public ChannelConfig()
        {
            Headers = new Dictionary<string, string>();
            ExchangeArgs = new Dictionary<string, object>();
        }

        public string Exchange { get; set; }
        public string ExchangeType { get; set; }
        public string RoutingKey { get; set; }
        public string QueueName { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, object> ExchangeArgs { get; set; }
        public Dictionary<string, object> QueueArgs { get; set; }
        public bool ManualAck { get; set; } = true;
        public string DeadLetterExchange { get; set; }
        public string DeadLetterKey { get; set; }

        public bool IsConfigDeadLetter => !string.IsNullOrEmpty(DeadLetterExchange) && !string.IsNullOrEmpty(DeadLetterKey);

        public string QueueOptions { get; set; }

        public bool IsRoutingMatch(string routing)
        {
            var routingPattern = RoutingKey.Replace("*", "[a-zA-Z0-9_^-]+");
            var regexPattern = new Regex(routingPattern, RegexOptions.IgnoreCase);

            return regexPattern.IsMatch(routing);
        }
    }
}