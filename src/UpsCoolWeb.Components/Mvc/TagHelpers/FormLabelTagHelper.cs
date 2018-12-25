using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace UpsCoolWeb.Components.Mvc
{
    [HtmlTargetElement("label", Attributes = "asp-for")]
    public class FormLabelTagHelper : TagHelper
    {
        public Boolean? Required { get; set; }

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            TagBuilder require = new TagBuilder("span");
            require.Attributes["class"] = "require";

            if (Required == true)
                require.InnerHtml.Append("*");

            if (Required == null && For.Metadata.IsRequired && For.Metadata.ModelType != typeof(Boolean))
                require.InnerHtml.Append("*");

            output.Content.AppendHtml(require);
        }
    }
}
