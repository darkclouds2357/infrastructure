using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.MessageBus.Settings
{
    public class MessageBusConfig
    {
        public IReadOnlyDictionary<string, MessageRouteConfig> DispatchedMessageRoutes { get; set; }
        public IReadOnlyDictionary<string, MessageRouteConfig> ListenedMessageRoutes { get; set; }
    }
}
