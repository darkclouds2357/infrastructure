using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alidu.Core.Common.Extensions
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
