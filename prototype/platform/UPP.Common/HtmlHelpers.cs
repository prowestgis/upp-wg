using Nancy.ViewEngines.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Upp.Common.Helpers
{
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

            var prefix = typeof(TModel).Name.ToLower();
            var inner = @"<input id=""{2}-{5}"" name=""{2}{3}"" type=""{6}"" class=""form-control"" value=""{1}""  {4} {7}/>";

            if (!String.IsNullOrEmpty(properties.suffix))
            {
                inner = @"<div class=""input-group"">" + inner + @"<span class=""input-group-addon"">" + properties.suffix  + @"</span></div>";
            }

            return htmlHelper.Raw(String.Format(@"
                <div class=""form-group row"">
                    <label class=""col-sm-2 control-label"">{0}</label>
                    <div class=""col-sm-8"">" +
                      inner +
                    @"</div>
                </div>
            ", properties.label, valueString, prefix, properties.id, properties.readOnly ? "readonly": string.Empty, properties.label.ToLower().Replace(" ","-"), properties.inputType, properties.range.AsInputProperties()));
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

            public string AsInputProperties(){
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
