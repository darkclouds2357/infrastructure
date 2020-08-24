using Newtonsoft.Json.Linq;
using Pluralize.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Alidu.Core.Domain
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }
            var regex = new Regex("[A-Z]", RegexOptions.Compiled);
            return regex.Replace(input, "_$&").Substring(1).ToLower();
        }

        public static string ToPluralizerSnakeCase(this string input)
        {
            var pluralizer = new Pluralizer();

            var snakeCase = input.ToSnakeCase();
            return pluralizer.Pluralize(snakeCase);
        }

        public static bool IsEmail(this string email)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                return false;

            var regex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            var match = regex.Match(email);
            if (match.Success == false)
                return false;

            return true;
        }

        public static bool IsValidJson(this string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (Exception) //some other exception
                {
                    return false;
                }
            }
            return false;
        }
    }
}
