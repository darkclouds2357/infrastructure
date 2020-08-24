using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.Common.Exceptions
{
    public class DomainException : Exception
    {
        public string[] Messages { get; }

        public DomainException()
        {
            Messages = new string[] { };
        }

        public DomainException(params string[] messages)
            : base(string.Join(Environment.NewLine, messages))
        {
            Messages = messages ?? new string[] { };
        }

        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
            Messages = new string[] { message };
        }

        public DomainException(string[] messages, Exception innerException)
            : base(string.Join(Environment.NewLine, messages, innerException))
        {
            Messages = messages ?? new string[] { };
        }
    }
}
