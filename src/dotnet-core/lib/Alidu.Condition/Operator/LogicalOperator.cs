using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Alidu.Condition.Operator
{
    public class LogicalOperator
    {
        public enum LogicalOperatorEnum
        {
            And = 1,
            Or = 2,
        }

        public static LogicalOperator And = new LogicalOperator(LogicalOperatorEnum.And, nameof(And).ToLower(), "&&");
        public static LogicalOperator Or = new LogicalOperator(LogicalOperatorEnum.Or, nameof(Or).ToLower(), "||");

        public static IReadOnlyList<LogicalOperator> List() => new[] { And, Or };

        private static readonly IReadOnlyDictionary<int, Func<Expression, Expression, BinaryExpression>> _idToExpression = new Dictionary<int, Func<Expression, Expression, BinaryExpression>>
        {
            [And.Operator] = Expression.AndAlso,
            [Or.Operator] = Expression.OrElse,
        };

        public string Name { get; }
        public string Symbol { get; }
        public int Operator { get; }
        public Func<Expression, Expression, BinaryExpression> LogicalOperatorExpression => _idToExpression[Operator];

        public LogicalOperator(LogicalOperatorEnum operatorEnum, string name, string symbol)
        {
            Name = name;
            Symbol = symbol;
            Operator = (int)operatorEnum;
        }

        public override string ToString() => Symbol;

        public static LogicalOperator FromName(string name)
        {
            var logicalOperator = List().SingleOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            return logicalOperator ?? throw new Exception($"Possible values for LogicalOperator: {string.Join(",", List().Select(s => s.Name))}");
        }

        public static LogicalOperator FromSymbol(string symbol)
        {
            var logicalOperator = List().SingleOrDefault(s => s.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));

            return logicalOperator ?? throw new Exception($"Possible values for CompareOperator: {string.Join(",", List().Select(s => s.Symbol))}");
        }

        public static LogicalOperator From(LogicalOperatorEnum @operator)
        {
            var logicalOperator = List().SingleOrDefault(s => s.Operator == (int)@operator);

            return logicalOperator ?? throw new Exception($"Possible values for LogicalOperator: {string.Join(",", List().Select(s => s.Name))}");
        }
    }
}