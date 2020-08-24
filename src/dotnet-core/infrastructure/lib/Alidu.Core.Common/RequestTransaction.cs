using Alidu.Core.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.Common
{
    public class RequestTransaction : IRequestTransaction
    {
        public RequestTransaction(string commandId)
        {
            CommandId = commandId;
        }
        public string CommandId { get; private set; }

        public void SetCommandId(string commandId) => CommandId = commandId;
    }
}
