using Alidu.Condition.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Alidu.Condition
{
    public static class ComparerExtension
    {
        public static IEnumerable<CheckValue> GetCheckValues<T>(this T comparer) where T : class
        {
            var type = comparer.GetType();

            var properties = type.GetProperties().Where(t => t.CustomAttributes.Any(c => c.AttributeType.IsSubclassOf(typeof(ComparerAttribute))));

            var result = new List<CheckValue>();

            foreach (var property in properties)
            {
                var value = property.GetValue(comparer);
                if (value == null)
                    continue;
                if (property.CustomAttributes.Any(a => a.AttributeType == typeof(ComparerPropertyAttribute)))
                {
                    var propertyAttribute = (ComparerPropertyAttribute)Attribute.GetCustomAttribute(property, typeof(ComparerPropertyAttribute));
                    result.Add(new CheckValue(propertyAttribute.Parameter, value.ToString(), value.GetType()));
                }
                if (property.CustomAttributes.Any(a => a.AttributeType == typeof(ComparerSourceAttribute)) && !(value is string))
                {
                    if (value is IEnumerable enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            result.AddRange(item.GetCheckValues());
                        }
                    }
                    else
                    {
                        result.AddRange(value.GetCheckValues());
                    }
                }
            }

            result = result.GroupBy(c => c.Parameter).Select(c => new CheckValue(c.Key, string.Join(";", c.Select(v => v.Value)), c.Select(t => t.Type).FirstOrDefault())).ToList();

            return result;
        }

        public static bool ValidateValues(this IEnumerable<CompareSource> sources, IEnumerable<CheckValue> checkValues)
        {
            var parameters = new List<ParameterExpression>();
            var verifyObjects = new List<object>();
            Expression bodyExpression = null;

            sources = sources.GroupBy(s => s.ParameterName).Select(sourceGroup =>
            {
                var compareValues = sourceGroup.SelectMany(source => source.CompareValues);

                return new CompareSource(sourceGroup.Key, compareValues.ToArray(), sourceGroup.FirstOrDefault().PreviousLogicalOperatorSymbol);
            }).ToList();

            foreach (var source in sources)
            {
                var value = checkValues.FirstOrDefault(v => v.Parameter == source.ParameterName);
                if (value == null)
                    return false;
                var conditionValue = ConditionType.GetConditionValue(value.ValueObjects, source.ValueType);
                var valueObject = source.DeserializeObject(conditionValue);
                verifyObjects.Add(valueObject);

                var (Parameter, BodyExpression) = source.GetCompareExpression();
                if (bodyExpression == null)
                    bodyExpression = BodyExpression;
                else
                    bodyExpression = source.PreviousLogicalOperator.LogicalOperatorExpression(bodyExpression, BodyExpression);
                parameters.Add(Parameter);
            }

            var lamda = Expression.Lambda(bodyExpression, parameters.ToArray());
            var validateFunc = lamda.Compile();
            var result = (bool)validateFunc.DynamicInvoke(verifyObjects.ToArray());

            return result;
        }

        public static Dictionary<string, bool> ValidateValuesEach(this IEnumerable<CompareSource> sources, IEnumerable<CheckValue> checkValues)
        {
            var result = new Dictionary<string, bool>();
            foreach (var rule in sources)
            {
                var value = checkValues.FirstOrDefault(v => v.Parameter == rule.ParameterName);
                result[rule.ParameterName] = value == null ? false : rule.ValidateValue(value);
            }
            return result;
        }

        public static bool ValidateValue(this CompareSource source, CheckValue checkValue)
        {
            if (checkValue.Parameter != source.ParameterName)
                return false;
            var conditionValue = ConditionType.GetConditionValue(checkValue.ValueObjects, source.ValueType);
            var valueObject = source.DeserializeObject(conditionValue);
            var (Parameter, BodyExpression) = source.GetCompareExpression();
            var lamda = Expression.Lambda(BodyExpression, Parameter);
            var validateFunc = lamda.Compile();
            return (bool)validateFunc.DynamicInvoke(valueObject);
        }
    }
}
