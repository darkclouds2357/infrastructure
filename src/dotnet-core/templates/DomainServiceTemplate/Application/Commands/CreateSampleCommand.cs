using Alidu.CQRS;
using Alidu.MessageBus.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
