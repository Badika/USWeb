using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UpsCoolWeb.Components.Mvc
{
    [HtmlTargetElement("input", Attributes = "asp-for,asp-placeholder")]
    public class PlaceholderTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add("placeholder", For.Metadata.DisplayName);
        }
    }
}
