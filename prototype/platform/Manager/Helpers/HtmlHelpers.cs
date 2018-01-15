using Nancy.ViewEngines.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Helpers
{
    public static class HtmlHelpers
    {
        public static IHtmlString GroupItem<TModel, TValue>(this Nancy.ViewEngines.Razor.HtmlHelpers<TModel> htmlHelper, TValue value, string label = "")
        {
            return htmlHelper.Raw(String.Format(@"
                <div class=""form-group row"">
                    <label class=""col-sm-2 control-label"">{0}</label>
                    <div class=""col-sm-8"">
                        <input type=""text"" class=""form-control"" value=""{1}"" />
                    </div>
                </div>
            ", label, value));
        }
    }
}
