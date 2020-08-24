using Alidu.Core.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.MessageBus.InMemory
{
    public class QueueMessage
    {

        public QueueMessage(object payload, IRequestCredential credential, IRequestChannel channel, IRequestTransaction transaction)
        {
            Payload = JsonConvert.SerializeObject(payload);
            Credential = credential;
            Channel = channel;
            Transaction = transaction;
            CommandId = transaction?.CommandId ?? Guid.NewGuid().ToString();
        }

        public string Payload { get; }
        public IRequestCredential Credential { get; }
        public IRequestChannel Channel { get; }
        public IRequestTransaction Transaction { get; }
        public string CommandId { get; }
    }
}
