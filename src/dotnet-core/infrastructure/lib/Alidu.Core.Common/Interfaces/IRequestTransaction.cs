using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.Common.Interfaces
{
    public interface IRequestTransaction
    {
        string CommandId { get; }
        void SetCommandId(string commandId);
    }
}
