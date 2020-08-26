﻿using System.Collections.Generic;

namespace Alidu.MessageBus.Settings
{
    public class MessageBusConfig
    {
        public IReadOnlyDictionary<string, MessageRouteConfig> DispatchedMessageRoutes { get; set; }
        public IReadOnlyDictionary<string, MessageRouteConfig> ListenedMessageRoutes { get; set; }
    }
}