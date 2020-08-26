using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Common.Exceptions
{
    public class HeaderException : Exception
    {
        public string[] Messages { get; }

        public HeaderException()
        {
            Messages = new string[] { };
        }

        public HeaderException(params string[] messages)
            : base(string.Join(Environment.NewLine, messages))
        {
            Messages = messages ?? new string[] { };
        }

        public HeaderException(string message, Exception innerException)
            : base(message, innerException)
        {
            Messages = new string[] { message };
        }

        public HeaderException(string[] messages, Exception innerException)
            : base(string.Join(Environment.NewLine, messages, innerException))
        {
            Messages = messages ?? new string[] { };
        }
    }
}
