using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public string[] Messages { get; }

        public NotFoundException()
        {
            Messages = new string[] { };
        }

        public NotFoundException(params string[] messages)
            : base(string.Join(Environment.NewLine, messages))
        {
            Messages = messages ?? new string[] { };
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            Messages = new string[] { message };
        }

        public NotFoundException(string[] messages, Exception innerException)
            : base(string.Join(Environment.NewLine, messages, innerException))
        {
            Messages = messages ?? new string[] { };
        }
    }
}
