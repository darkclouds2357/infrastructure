using Alidu.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.MessageBus.InMemory
{
    public class QueueMessage
    {

        public QueueMessage(object payload, IRequestCredential credential, IRequestChannel channel, IRequestCommand command)
        {
            Payload = JsonConvert.SerializeObject(payload);
            Credential = credential;
            Channel = channel;
            Command = command;
            CommandId = command?.CommandId ?? Guid.NewGuid().ToString();
        }

        public string Payload { get; }
        public IRequestCredential Credential { get; }
        public IRequestChannel Channel { get; }
        public IRequestCommand Command { get; }
        public string CommandId { get; }
    }
}
