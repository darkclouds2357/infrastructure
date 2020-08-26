using MediatR;

namespace Alidu.Core.Domain.Interfaces
{
    public interface IDomainEvent : INotification
    {
        int Version { get; set; }
    }
}