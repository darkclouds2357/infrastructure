using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Condition.Attributes
{
    public class ComparerAttribute : Attribute
    {
    }

    public class ComparerSourceAttribute : ComparerAttribute
    {
    }

    public class ComparerPropertyAttribute : ComparerAttribute
    {
        public string Parameter { get; set; }
    }
}
