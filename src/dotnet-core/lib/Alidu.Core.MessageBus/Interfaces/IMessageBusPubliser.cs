using Alidu.Core.Domain;

namespace Alidu.Core.MessageBus.Interfaces
{
    public interface IMessageBusPubliser
    {
        string Publish(IntegrationMessage message);

        string PublishDynamic(dynamic message);
    }
}
