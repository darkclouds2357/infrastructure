using Alidu.Core.MessageBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Alidu.Core.MessageBus
{
    public interface IMessageHandler<in TEvent> : IMessageHandler
        where TEvent : MessagePayload
    {
        Task Handle(string correlationId, TEvent @event);
    }

    public interface IMessageHandler
    {
    }
}
