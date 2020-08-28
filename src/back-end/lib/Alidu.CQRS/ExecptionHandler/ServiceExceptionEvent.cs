using Alidu.MessageBus.Abstractions;
using MediatR;

namespace Alidu.CQRS.ExecptionHandler
{
    public class ServiceExceptionEvent : BaseMessage, INotification
    {
        public ServiceExceptionEvent() : base(EventEnum.SERVICE_HAS_EXCEPTION_EVENT)
        {
        }

        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}