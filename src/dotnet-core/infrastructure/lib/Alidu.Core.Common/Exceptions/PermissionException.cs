using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.Common.Exceptions
{
    public class PermissionException : Exception
    {
        public string[] Messages { get; }

        public PermissionException()
        {
            Messages = new string[] { };
        }

        public PermissionException(params string[] messages)
            : base(string.Join(Environment.NewLine, messages))
        {
            Messages = messages ?? new string[] { };
        }

        public PermissionException(string message, Exception innerException)
            : base(message, innerException)
        {
            Messages = new string[] { message };
        }

        public PermissionException(string[] messages, Exception innerException)
            : base(string.Join(Environment.NewLine, messages, innerException))
        {
            Messages = messages ?? new string[] { };
        }
    }
}
