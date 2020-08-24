using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.MessageBus.Abstractions
{
    public abstract class MessagePayload
    {
        protected MessagePayload()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            MessageName = this.GetType().Name;
        }

        public MessagePayload(string messageName) : this()
        {
            MessageName = messageName;
        }

        [JsonConstructor]
        public MessagePayload(Guid id, DateTime createDate, string messageName)
        {
            Id = id;
            CreationDate = createDate;
            MessageName = messageName;
        }

        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public DateTime CreationDate { get; private set; }

        [JsonProperty]
        public string MessageName { get; private set; }

        [JsonProperty]
        public string TransactionId { get; protected set; }

        public void SetTransactionId(string transactionId) => TransactionId = transactionId;
    }
}
