using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.Domain
{
    public abstract class IntegrationMessage : INotification
    {
        public IntegrationMessage()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            MessageName = this.GetGenericTypeName();
        }
        public IntegrationMessage(string messageName) : this()
        {
            MessageName = messageName;
        }

        [JsonConstructor]
        public IntegrationMessage(Guid id, DateTime createDate, string messageName)
        {
            Id = id;
            CreationDate = createDate;
            MessageName = messageName;
        }


        public Guid Id { get; private set; }
        public DateTime CreationDate { get; private set; }
        public int Version { get; internal set; }
        public string MessageName { get; private set; }
        public Guid TransactionId { get; private set; }

        public void SetTransactionId(Guid transactionId) => TransactionId = transactionId;
    }
}
