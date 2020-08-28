using MediatR;

namespace Alidu.Core.Domain.Interfaces
{
    public interface IAggregateEvent : INotification
    {
        string AggregateId { get; }
        string EventName { get; }
        int Version { get; }

        void SetAggregateVersion(string aggregateId, int version);
    }
}