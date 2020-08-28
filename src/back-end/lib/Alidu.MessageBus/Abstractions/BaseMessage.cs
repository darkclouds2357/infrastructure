using Alidu.Core.Domain;
using Alidu.Core.Domain.Interfaces;
using Newtonsoft.Json;
using System;

namespace Alidu.MessageBus.Abstractions
{
    public abstract class BaseMessage
    {
        public BaseMessage()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            MessageName = this.GetGenericTypeName();
        }

        public BaseMessage(string messageName) : this()
        {
            MessageName = messageName;
        }

        [JsonConstructor]
        public BaseMessage(Guid id, DateTime createDate, string messageName)
        {
            Id = id;
            CreationDate = createDate;
            MessageName = messageName;
        }

        public Guid Id { get; private set; }
        public DateTime CreationDate { get; private set; }
        public string MessageName { get; private set; }
        public Guid TransactionId { get; private set; }

        public void SetTransactionId(Guid transactionId) => TransactionId = transactionId;
    }
}