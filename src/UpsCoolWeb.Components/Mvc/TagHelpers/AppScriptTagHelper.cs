using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace UpsCoolWeb.Components.Mvc
{
    [HtmlTargetElement("script", Attributes = "action")]
    public class AppScriptTagHelper : TagHelper
    {
        public override Int32 Order => -2000;

        public String Action { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        private IHostingEnvironment Environment { get; }

        private static ConcurrentDictionary<String, String> Scripts { get; }

        static AppScriptTagHelper()
        {
            Scripts = new ConcurrentDictionary<String, String>();
        }
        public AppScriptTagHelper(IHostingEnvironment environment)
        {
            Environment = environment;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            String path = FormPath();

            if (!Scripts.ContainsKey(path))
            {
                Scripts[path] = null;

                if (ScriptsAvailable(path))
                    Scripts[path] = new UrlHelper(ViewContext).Content("~/scripts/application/" + path);
            }

            if (Scripts[path] == null)
                output.TagName = null;
            else
                output.Attributes.SetAttribute("src", Scripts[path]);
        }

        private Boolean ScriptsAvailable(String path)
        {
            return File.Exists(Path.Combine(Environment.WebRootPath, "scripts/application/" + path));
        }
        private String FormPath()
        {
            RouteValueDictionary route = ViewContext.RouteData.Values;
            String extension = Environment.IsDevelopment() ? ".js" : ".min.js";

            return ((route["Area"] == null ? null : route["Area"] + "/") + route["controller"] + "/" + Action + extension).ToLower();
        }
    }
}
