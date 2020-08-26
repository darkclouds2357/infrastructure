using System;
using System.Collections.Generic;

namespace Alidu.Condition
{
    public class ConditionType
    {
        public const string DOUBLE = "double";
        public const string INTEGER = "int";
        public const string STRING = "string";
        public const string DATETIME = "datetime";
        public const string YEAR = "year";
        public const string MONTH = "month";
        public const string DAY = "day";
        public const string DAY_OF_WEEK = "day_of_week";
        public const string DATE = "date";
        public const string HOUR = "hour";

        public static IReadOnlyDictionary<string, Type> ConditionSytemType = new Dictionary<string, Type>
        {
            [DOUBLE] = typeof(double),
            [INTEGER] = typeof(int),
            [STRING] = typeof(string),
            [DATETIME] = typeof(DateTime),
            [YEAR] = typeof(int),
            [MONTH] = typeof(int),
            [DATE] = typeof(DateTime),
            [DAY] = typeof(int),
            [DAY_OF_WEEK] = typeof(int),
            [HOUR] = typeof(int)
        };

        public static string GetConditionValue(IEnumerable<object> value, string conditionType)
        {
            var result = new List<string>();
            if (!ConditionSytemType.ContainsKey(conditionType))
                return string.Empty;
            var systemType = ConditionSytemType[conditionType];

            foreach (var item in value)
            {
                if (item is DateTime dateTime)
                {
                    switch (conditionType)
                    {
                        case YEAR:
                            result.Add(dateTime.Year.ToString());
                            break;

                        case MONTH:
                            result.Add(dateTime.Month.ToString());
                            break;
                        //case DATE:
                        //    result.Add(dateTime.Date.ToString("yyyy-MM-dd"));
                        //    break;
                        case DAY:
                            result.Add(dateTime.Day.ToString());
                            break;

                        case DAY_OF_WEEK:
                            result.Add(((int)dateTime.DayOfWeek).ToString());
                            break;

                        case HOUR:
                            result.Add(dateTime.Hour.ToString());
                            break;

                        default:
                            result.Add(dateTime.ToString());
                            break;
                    }
                }
                else if (item.GetType() == systemType)
                {
                    result.Add(item.ToString());
                }
            }

            return string.Join(";", result);
        }
    }
}