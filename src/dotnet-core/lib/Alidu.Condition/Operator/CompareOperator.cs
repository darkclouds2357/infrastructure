using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Alidu.Condition.Operator
{
    public enum CompareOperatorEnum
    {
        Equal = 1,
        GreaterThan = 2,
        LessThan = 4,
        NotEqual = 8
    }

    public class CompareOperator
    {
        public static CompareOperator Equal = new CompareOperator(CompareOperatorEnum.Equal, nameof(Equal), "==");
        public static CompareOperator GreaterThan = new CompareOperator(CompareOperatorEnum.GreaterThan, nameof(GreaterThan), ">");
        public static CompareOperator GreaterThanOrEqual = new CompareOperator(CompareOperatorEnum.GreaterThan | CompareOperatorEnum.Equal, nameof(GreaterThanOrEqual), ">=");
        public static CompareOperator LessThan = new CompareOperator(CompareOperatorEnum.LessThan, nameof(LessThan), "<");
        public static CompareOperator LessThanOrEqual = new CompareOperator(CompareOperatorEnum.LessThan | CompareOperatorEnum.Equal, nameof(LessThan), "<=");
        public static CompareOperator NotEqual = new CompareOperator(CompareOperatorEnum.NotEqual, nameof(NotEqual), "!=");

        public static IReadOnlyList<CompareOperator> List() => new[] { Equal, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual, NotEqual };

        private static readonly IReadOnlyDictionary<int, Func<Expression, Expression, BinaryExpression>> _idToExpression = new Dictionary<int, Func<Expression, Expression, BinaryExpression>>
        {
            [Equal.Operator] = Expression.Equal,
            [GreaterThan.Operator] = Expression.GreaterThan,
            [LessThan.Operator] = Expression.LessThan,
            [GreaterThan.Operator | Equal.Operator] = Expression.GreaterThanOrEqual,
            [LessThan.Operator | Equal.Operator] = Expression.LessThanOrEqual,
            [NotEqual.Operator] = Expression.NotEqual
        };

        public string Name { get; }
        public string Symbol { get; }
        public int Operator { get; }

        [JsonIgnore]
        public Func<Expression, Expression, BinaryExpression> OperatorExpression => _idToExpression[Operator];

        public CompareOperator(int operatorEnum)
        {
            Operator = operatorEnum;
        }

        public CompareOperator(CompareOperatorEnum operatorEnum, string name, string symbol) : this((int)operatorEnum)
        {
            Name = name;
            Symbol = symbol;
        }

        public override string ToString() => Symbol;

        public static CompareOperator FromName(string name)
        {
            var compareOperator = List().SingleOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            return compareOperator ?? throw new Exception($"Possible values for CompareOperator: {string.Join(",", List().Select(s => s.Name))}");
        }

        public static CompareOperator FromSymbol(string symbol)
        {
            var compareOperator = List().SingleOrDefault(s => s.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));

            return compareOperator ?? throw new Exception($"Possible values for CompareOperator: {string.Join(",", List().Select(s => s.Symbol))}");
        }

        public static CompareOperator From(CompareOperatorEnum @operator)
        {
            var compareOperator = List().SingleOrDefault(s => s.Operator == (int)@operator);

            return compareOperator ?? throw new Exception($"Possible values for CompareOperator: {string.Join(",", List().Select(s => s.Name))}");
        }

        public static CompareOperator operator |(CompareOperator first, CompareOperator second) => new CompareOperator(first.Operator | second.Operator);

        public static CompareOperator operator &(CompareOperator first, CompareOperator second) => new CompareOperator(first.Operator & second.Operator);
    }
}
