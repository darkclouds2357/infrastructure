using System;

namespace Alidu.Common.Exceptions
{
    public class TokenException : Exception
    {
        public string[] Messages { get; }

        public TokenException()
        {
            Messages = new string[] { };
        }

        public TokenException(params string[] messages)
            : base(string.Join(Environment.NewLine, messages))
        {
            Messages = messages ?? new string[] { };
        }

        public TokenException(string message, Exception innerException)
            : base(message, innerException)
        {
            Messages = new string[] { message };
        }

        public TokenException(string[] messages, Exception innerException)
            : base(string.Join(Environment.NewLine, messages, innerException))
        {
            Messages = messages ?? new string[] { };
        }
    }
}