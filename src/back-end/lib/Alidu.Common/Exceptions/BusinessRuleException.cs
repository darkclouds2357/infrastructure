using System;

namespace Alidu.Common.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public string[] Messages { get; }

        public BusinessRuleException()
        {
            Messages = new string[] { };
        }

        public BusinessRuleException(params string[] messages)
            : base(string.Join(Environment.NewLine, messages))
        {
            Messages = messages ?? new string[] { };
        }

        public BusinessRuleException(string message, Exception innerException)
            : base(message, innerException)
        {
            Messages = new string[] { message };
        }

        public BusinessRuleException(string[] messages, Exception innerException)
            : base(string.Join(Environment.NewLine, messages, innerException))
        {
            Messages = messages ?? new string[] { };
        }
    }
}