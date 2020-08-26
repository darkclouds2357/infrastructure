using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Alidu.Condition.Operator
{
    public class ArrayOperator
    {
        public enum ArrayOperatorEnum
        {
            In = 1,
            All = 2,
        }

        public static ArrayOperator In = new ArrayOperator(ArrayOperatorEnum.In, nameof(In), nameof(Enumerable.Any));
        public static ArrayOperator All = new ArrayOperator(ArrayOperatorEnum.All, nameof(All), nameof(Enumerable.All));

        public static IReadOnlyList<ArrayOperator> List() => new[] { In, All };

        public string Name { get; }
        public string EnumerableMethod { get; }
        public int Operator { get; }

        public ArrayOperator(ArrayOperatorEnum operatorEnum, string name, string enumerableMethod)
        {
            Name = name;
            EnumerableMethod = enumerableMethod;
            Operator = (int)operatorEnum;
        }

        public Expression GetCompareExpression(IEnumerable<string> sources, ParameterExpression parameter)
        {
            var compareItemParameter = Expression.Parameter(typeof(string), "item");
            var sourcesConstant = Expression.Constant(sources);

            // parameter.Contains(item)
            MethodCallExpression compareExpressions = Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] { typeof(string) }, parameter, compareItemParameter); ;

            // item => parameter.Contains(item)
            var lamdaExpression = Expression.Lambda(compareExpressions, compareItemParameter);

            // sources.Method(item => parameter.Contains(item))
            return Expression.Call(typeof(Enumerable), EnumerableMethod, new[] { typeof(string) }, sourcesConstant, lamdaExpression);
        }

        public override string ToString() => Name;

        public static ArrayOperator FromName(string name)
        {
            var logicalOperator = List().SingleOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            return logicalOperator ?? throw new Exception($"Possible values for ArrayOperator: {string.Join(",", List().Select(s => s.Name))}");
        }

        public static ArrayOperator From(ArrayOperatorEnum @operator)
        {
            var logicalOperator = List().SingleOrDefault(s => s.Operator == (int)@operator);

            return logicalOperator ?? throw new Exception($"Possible values for ArrayOperator: {string.Join(",", List().Select(s => s.Name))}");
        }
    }
}