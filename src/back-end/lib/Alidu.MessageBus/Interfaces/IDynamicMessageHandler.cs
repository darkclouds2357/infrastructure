using System.Threading.Tasks;

namespace Alidu.MessageBus.Interfaces
{
    public interface IDynamicMessageHandler
    {
        Task Handle(string correlationId, dynamic message);
    }
}