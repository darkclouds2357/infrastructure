using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Alidu.Core.MessageBus
{
    public interface IDynamicMessageHandler
    {
        Task Handle(string correlationId, dynamic message);
    }
}
