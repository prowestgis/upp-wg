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
        public static IHtmlString GroupItem<TModel, TValue>(this Nancy.ViewEngines.Razor.HtmlHelpers<TModel> htmlHelper, TValue value, string id, string label = "", bool readOnly = false)
        {
            var prefix = typeof(TModel).Name.ToLower();
            
            return htmlHelper.Raw(String.Format(@"
                <div class=""form-group row"">
                    <label class=""col-sm-2 control-label"">{0}</label>
                    <div class=""col-sm-8"">
                        <input id=""{2}-{5}"" name=""{2}{3}"" type=""text"" class=""form-control"" value=""{1}"" {4}/>
                    </div>
                </div>
            ", label, value, prefix, id, readOnly ? "readonly": string.Empty, label.ToLower().Replace(" ","-")));
        }
    }
}
