using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Alidu.Condition
{
    public class CheckValue
    {
        public CheckValue(string parameter, string value, Type type)
        {
            Parameter = parameter;
            Value = value;
            Type = type;
        }

        public string Parameter { get; }
        public string Value { get; }
        public Type Type { get; }

        public IEnumerable<object> ValueObjects
        {
            get
            {
                var typeConvert = TypeDescriptor.GetConverter(Type);
                var values = Value.Split(";");
                foreach (var item in values)
                {
                    yield return typeConvert.ConvertFromInvariantString(item);
                }
            }
        }
    }
}
