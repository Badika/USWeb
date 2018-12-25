using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace UpsCoolWeb.Components.Mvc
{
    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class FormInputTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (For.Metadata.ModelType == typeof(Boolean))
                return;

            if (output.Attributes["autocomplete"] == null)
                output.Attributes.Add("autocomplete", "off");

            if (output.Attributes["class"] == null)
                output.Attributes.Insert(0, new TagHelperAttribute("class", "form-control"));
            else
                output.Attributes.SetAttribute("class", "form-control " + output.Attributes["class"].Value);
        }
    }
}
