using Alidu.Common.Interfaces;
using Newtonsoft.Json;
using System;

namespace Alidu.MessageBus.InMemory
{
    public class QueueMessage
    {
        public QueueMessage(object payload, IRequestHeader header)
        {
            Payload = JsonConvert.SerializeObject(payload);
            Header = header;
            CommandId = header?.Command?.CommandId ?? Guid.NewGuid().ToString();
        }

        public string Payload { get; }
        public IRequestHeader Header { get; }
        public string CommandId { get; }
    }
}