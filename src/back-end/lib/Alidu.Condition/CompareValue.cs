using Alidu.Condition.Operator;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Alidu.Condition
{
    public class CompareValue
    {
        private readonly object _valueObject;
        private readonly string[] _valueArray;

        public CompareValue()
        {
        }

        public CompareValue(string value, int index, Rule rule, string previousLogicalOperatorSymbol) : this()
        {
            Value = value;
            Index = index;
            Rule = rule;
            PreviousLogicalOperatorSymbol = previousLogicalOperatorSymbol;
            if (!IsArray)
            {
                _valueObject = GetValue(Rule.SystemType, value);
            }
            else
            {
                _valueArray = value.Split(';').OrderBy(v => v).ToArray();
            }
        }

        public string PreviousLogicalOperatorSymbol { get; }
        public string Value { get; }
        public int Index { get; }

        [JsonIgnore]
        public LogicalOperator PreviousLogicalOperator => !string.IsNullOrWhiteSpace(PreviousLogicalOperatorSymbol) ? LogicalOperator.FromSymbol(PreviousLogicalOperatorSymbol) : null;

        public Rule Rule { get; }

        public bool IsArray => Rule.ArrayOperator != null;

        public Expression GetCompareExpression(ParameterExpression parameter)
        {
            return !IsArray ? GetCompareOperatorExpression(parameter) : Rule.ArrayOperator.GetCompareExpression(_valueArray, parameter);
        }

        private BinaryExpression GetCompareOperatorExpression(Expression left)
        {
            if (_valueObject == null)
                throw new Exception("Should Parse Value to Correct Type");
            var right = Expression.Constant(_valueObject);
            var operatorExpression = Rule.Operator.OperatorExpression;
            return operatorExpression(left, right);
        }

        private object GetValue(Type type, string value)
        {
            var typeConvert = TypeDescriptor.GetConverter(type);
            return typeConvert.ConvertFromInvariantString(value);
        }
    }
}