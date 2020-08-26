using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Common.Interfaces
{
    public interface IRequestCommand
    {
        string CommandId { get; }
        void SetCommandId(string commandId);
    }
}
