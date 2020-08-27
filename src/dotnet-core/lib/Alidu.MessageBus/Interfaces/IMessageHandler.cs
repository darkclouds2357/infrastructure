using Alidu.MessageBus.Abstractions;
using System.Threading.Tasks;

namespace Alidu.MessageBus.Interfaces
{
    public interface IMessageHandler<in TEvent> : IMessageHandler
        where TEvent : BaseMessage
    {
        Task Handle(string correlationId, TEvent @event);
    }

    public interface IMessageHandler
    {
    }
}