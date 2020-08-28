using Alidu.Condition.Operator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Alidu.Condition
{
    public class CompareSource
    {
        protected CompareSource()
        {
            CompareValues = new List<CompareValue>();
        }

        public CompareSource(string parameterName, IReadOnlyList<CompareValue> compareValues, string previousLogicalOperatorSymbol) : this()
        {
            ParameterName = parameterName;
            CompareValues = compareValues;
            PreviousLogicalOperatorSymbol = previousLogicalOperatorSymbol;
        }

        public string ParameterName { get; }
        public IReadOnlyList<CompareValue> CompareValues { get; }

        public string PreviousLogicalOperatorSymbol { get; }

        [JsonIgnore]
        public LogicalOperator PreviousLogicalOperator => !string.IsNullOrWhiteSpace(PreviousLogicalOperatorSymbol) ? LogicalOperator.FromSymbol(PreviousLogicalOperatorSymbol) : LogicalOperator.And;

        private object GetValue(Type type, string value)
        {
            var typeConvert = TypeDescriptor.GetConverter(type);
            return typeConvert.ConvertFromInvariantString(value);
        }

        public object DeserializeObject(string value)
        {
            if (!CompareValues.Any(r => r.IsArray))
                return GetValue(SystemType, value);
            return value.Split(';');
        }

        public string ValueType
        {
            get
            {
                if (!CompareValues.Any())
                    return ConditionType.STRING;
                if (CompareValues.Any(r => r.IsArray))
                    return ConditionType.STRING;
                return CompareValues.Select(r => r.Rule.ValueType).FirstOrDefault();
            }
        }

        public Type SystemType
        {
            get
            {
                if (!CompareValues.Any())
                    return null;
                if (CompareValues.Any(r => r.IsArray))
                    return typeof(string[]);
                return CompareValues.Select(r => r.Rule.SystemType).FirstOrDefault();
            }
        }

        public (ParameterExpression Parameter, Expression Body) GetCompareExpression()
        {
            Expression body = null;
            var parameter = Expression.Parameter(SystemType, ParameterName.ToLower());
            var expression = CompareValues.OrderBy(t => t.Index).Aggregate(body, (current, next) =>
            {
                var compareExpression = next.GetCompareExpression(parameter);
                var logicalExpression = next.PreviousLogicalOperator != null ? next.PreviousLogicalOperator.LogicalOperatorExpression : LogicalOperator.And.LogicalOperatorExpression;
                if (current == null)
                    return compareExpression;
                return logicalExpression(current, compareExpression);
            });
            return (parameter, expression);
        }
    }
}