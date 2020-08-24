using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.Domain.Interfaces
{
    public interface IDomainEvent : INotification
    {
        public int Version { get; internal set; }
    }
}
