using Nancy.ViewEngines.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Upp.Common.Helpers
{
    public static class FormatConverter
    {
        /// <summary>
        /// Converts a string from CamelCase to hyphen-separated
        /// 
        /// see  http://stackoverflow.com/questions/1175208/elegant-python-function-to-convert-camelcase-to-camel-case
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CamelCaseToHyphenSeparated(string input)
        {
            var s = Regex.Replace(input, @"(.)([A-Z][a-z]+)", @"$1-$2");
            return Regex.Replace(s, @"([a-z0-9])([A-Z])", @"$1-$2").ToLowerInvariant().Replace(" ", string.Empty);
        }

        public static IEnumerable<int> AllIndexesOfAny(this string str, char[] anyOf)
        {
            int minIndex = str.IndexOfAny(anyOf);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOfAny(anyOf, minIndex + 1);
            }
        }

        /// <summary>
        /// Convert to camel case.  We are smart an change some_string into someString by captializing
        /// any character after an underscore.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Uppercase everything after underscores
            var sb = new StringBuilder(input);
            var special = input.AllIndexesOfAny(new[] { '_', '-' }).ToList();

            var last = input.Length - 1;
            foreach (var index in special.Where(idx => idx < last))
            {
                sb[index] = Char.ToUpperInvariant(sb[index]);
            }

            // Lowercase the first character (unless it is followed by another upper-case)
            if (sb.Length == 1 || !Char.IsUpper(sb[1]))
            {
                sb[0] = Char.ToLowerInvariant(sb[0]);
            }

            // Remove all spaces, underscores and hyphens
            return sb.ToString().Replace(" ", String.Empty).Replace("-", String.Empty).Replace("_", String.Empty);
        }

        public static string PrettyPrintCSharpType(Type t)
        {
            if (!t.IsGenericType)
            {
                return t.Name;
            }

            var args = t.GetGenericArguments().Select(PrettyPrintCSharpType);
            return String.Format("{0}<{1}>", String.Concat(t.Name.TakeWhile(x => x != '`')), String.Join(",", args));
        }
    }

    public static class HtmlHelpers
    {
        public static IHtmlString GroupItem<TModel, TValue>(this Nancy.ViewEngines.Razor.HtmlHelpers<TModel> htmlHelper, TValue value, GroupItemProperties properties)
        {
            // Handle certain format restrictions
            var valueString = String.Format("{0}", value);

            if (properties.inputType == GroupItemProperties.InputType.DATE && value is DateTime)
            {
                valueString = String.Format("{0:yyyy-MM-dd}", value);
            }

            if (properties.inputType == GroupItemProperties.InputType.DATE_TIME && value is DateTime)
            {
                valueString = String.Format("{0:yyyy-MM-ddThh:mm}", value);
            }

            var prefix = typeof(TModel).Name.ToCamelCase();
            var inner = @"<input id=""{2}.{3}"" name=""{2}.{3}"" type=""{6}"" class=""form-control"" value=""{1}""  {4} {7}/>";

            if (!String.IsNullOrEmpty(properties.suffix))
            {
                inner = @"<div class=""input-group"">" + inner + @"<span class=""input-group-addon"">" + properties.suffix + @"</span></div>";
            }

            return htmlHelper.Raw(String.Format(@"
                <div class=""form-group row"">
                    <label class=""col-sm-2 control-label"">{0}</label>
                    <div class=""col-sm-8"">" +
                      inner +
                    @"</div>
                </div>
            ", properties.label, valueString, prefix, properties.id.ToCamelCase(), properties.readOnly ? "readonly" : string.Empty, properties.label.ToLower().Replace(" ", "-"), properties.inputType, properties.range.AsInputProperties()));
        }
    }

    public class GroupItemProperties
    {
        public static class Suffixes
        {
            public const string POUNDS = "lbs";
            public const string INCHES = "in";
            public const string FEET = "ft";
            public const string MILES = "miles";
            public const string TONS = "tons";
        }

        public GroupItemProperties(string id)
        {
            this.id = id;
            label = string.Empty;
            readOnly = false;
            inputType = "text";
            range = new Range();
            suffix = null;
        }
        public string id { get; private set; }
        public string label { get; set; }
        public bool readOnly { get; set; }
        public string inputType { get; set; }
        public string suffix { get; set; }
        public Range range { get; set; }

        public class Range
        {
            public double? min { get; set; }
            public double? max { get; set; }
            public double? step { get; set; }

            public string AsInputProperties()
            {
                StringBuilder range = new StringBuilder();
                if (max != null)
                {
                    range.Append(@"max=""");
                    range.Append(max);
                    range.Append(@""" ");
                }
                if (min != null)
                {
                    range.Append(@"min=""");
                    range.Append(min);
                    range.Append(@""" ");
                }
                if (step != null)
                {
                    range.Append(@"step=""");
                    range.Append(step);
                    range.Append(@""" ");
                }
                return range.ToString();
            }
        }

        public class InputType
        {
            // HTML5 Input Types
            public const string COLOR = "color";
            public const string DATE = "date";
            public const string DATE_TIME = "datetime-local";
            public const string EMAIL = "email";
            public const string MONTH = "month";
            public const string NUMBER = "number";
            public const string RANGE = "range";
            public const string SEARCH = "search";
            public const string TEL = "tel";
            public const string TIME = "time";
            public const string URL = "url";
            public const string WEEK = "week";

            public const string CHECKBOX = "checkbox";

        }
    }
}
