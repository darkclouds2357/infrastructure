using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.MessageBus.Settings
{
    public class MessageRouteConfig
    {
        public string[] Channels { get; set; }
        public bool ManualAck { get; set; } = false;
        public IReadOnlyDictionary<string, string> PayloadSchema { get; set; }
    }
}
