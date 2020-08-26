using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.Domain.Interfaces
{
    public interface IDomainEvent : INotification
    {
        int Version { get; set; }
    }
}
