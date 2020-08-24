using Alidu.Core.MessageBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.MessageBus.Interfaces
{
    public interface IMessageBusPubliser
    {
        string Publish(MessagePayload message);

        string PublishDynamic(dynamic message);
    }
}
