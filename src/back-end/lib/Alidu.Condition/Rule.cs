using Alidu.Condition.Operator;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Alidu.Condition
{
    public class Rule
    {
        public Rule()
        {
        }

        public Rule(string parameterName, string description, string valueType, string operatorSymbol) : this()
        {
            ParameterName = parameterName;
            ValueType = valueType;
            OperatorSymbol = operatorSymbol;
            Description = description;
        }

        public string ParameterName { get; }
        public string Description { get; }
        public string ValueType { get; }
        public string OperatorSymbol { get; }
        public bool IsArray => !(string.IsNullOrWhiteSpace(OperatorSymbol) || !ArrayOperator.List().Any(a => a.Name.Equals(OperatorSymbol, StringComparison.OrdinalIgnoreCase)));

        [JsonIgnore]
        public ArrayOperator ArrayOperator => IsArray ? ArrayOperator.FromName(OperatorSymbol) : null;

        [JsonIgnore]
        public CompareOperator Operator => !IsArray ? CompareOperator.FromSymbol(OperatorSymbol ?? CompareOperator.Equal.Symbol) : null;

        [JsonIgnore]
        public Type SystemType
        {
            get
            {
                if (!ConditionType.ConditionSytemType.ContainsKey(ValueType))
                    throw new Exception($"Invalid Type {ValueType}");
                var type = ConditionType.ConditionSytemType[ValueType];
                return type;
            }
        }
    }
}