using System;
using System.Collections.Generic;
using System.Linq;

namespace Alidu.MessageBus.Settings
{
    public class MessageTypeConfig
    {
        public IReadOnlyDictionary<Type, string> DispatcherMessageTypes { get; set; }
        public IReadOnlyDictionary<Type, string> ListenerMessageTypes { get; set; }

        public IReadOnlyDictionary<Type, string> MessageTypes
        {
            get
            {
                var result = ListenerMessageTypes.Concat(DispatcherMessageTypes);

                return result.GroupBy(r => r.Key, r => r.Value).ToDictionary(r => r.Key, r => r.FirstOrDefault());
            }
        }

        public MessageTypeConfig()
        {
            DispatcherMessageTypes = new Dictionary<Type, string>();
            ListenerMessageTypes = new Dictionary<Type, string>();
        }
    }
}