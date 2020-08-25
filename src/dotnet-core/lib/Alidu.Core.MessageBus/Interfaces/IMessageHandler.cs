using Alidu.Core.Domain;
using System.Threading.Tasks;

namespace Alidu.Core.MessageBus.Interfaces
{
    public interface IMessageHandler<in TEvent> : IMessageHandler
        where TEvent : IntegrationMessage
    {
        Task Handle(string correlationId, TEvent @event);
    }

    public interface IMessageHandler
    {
    }
}
