using System;
using System.Linq;

namespace Alidu.Common.Extensions
{
    public static class ExceptionExtension
    {
        public static string GetAllMessages(this Exception exception)
        {
            var messages = exception.FromHierarchy(ex => ex.InnerException).Select(ex => ex.Message);
            return string.Join(Environment.NewLine, messages);
        }
    }
}