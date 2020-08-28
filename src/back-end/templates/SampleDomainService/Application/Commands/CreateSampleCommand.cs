using Alidu.MessageBus.Abstractions;
using MediatR;

namespace SampleDomainService.Application.Commands
{
    public class CreateSampleCommand : BaseMessage, IRequest
    {
        public CreateSampleCommand() : base(SampleCommandEnum.CREATE_NEW_SAMPLE_COMMAND)
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}