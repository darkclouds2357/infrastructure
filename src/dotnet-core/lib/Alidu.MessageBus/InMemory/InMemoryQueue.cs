using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.MessageBus.InMemory
{
    public class InMemoryQueue
    {

        private readonly Dictionary<string, Queue<QueueMessage>> _queues;

        public event EventHandler<string> MessageReceived;

        public InMemoryQueue()
        {
            _queues = new Dictionary<string, Queue<QueueMessage>>();
        }

        public void Publish(object sender, string channel, QueueMessage queueMessage)
        {
            GetQueues(channel).Enqueue(queueMessage);

            MessageReceived?.Invoke(sender, channel);

        }

        public Queue<QueueMessage> GetQueues(string channel)
        {
            if (!_queues.ContainsKey(channel))
                _queues[channel] = new Queue<QueueMessage>();

            return _queues[channel];
        }
    }
}
