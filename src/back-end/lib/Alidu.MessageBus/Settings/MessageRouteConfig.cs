using System.Collections.Generic;

namespace Alidu.MessageBus.Settings
{
    public class MessageRouteConfig
    {
        public string[] Channels { get; set; }
        public bool ManualAck { get; set; } = false;
        public IReadOnlyDictionary<string, string> PayloadSchema { get; set; }
    }
}