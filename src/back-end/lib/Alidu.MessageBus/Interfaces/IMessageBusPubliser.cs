using Alidu.MessageBus.Abstractions;

namespace Alidu.MessageBus.Interfaces
{
    public interface IMessageBusPubliser
    {
        string Publish(BaseMessage message);

        string PublishDynamic(dynamic message);
    }
}