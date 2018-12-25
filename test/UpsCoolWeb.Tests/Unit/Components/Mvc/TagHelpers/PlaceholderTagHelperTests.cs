using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;
using System;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class PlaceholderTagHelperTests
    {
        #region Process(TagHelperContext context, TagHelperOutput output)

        [Fact]
        public void Process_Placeholder()
        {
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
            TagHelperOutput output = new TagHelperOutput("input", new TagHelperAttributeList(), (useCache, encoder) => null);
            PlaceholderTagHelper helper = new PlaceholderTagHelper { For = new ModelExpression("Total", new ModelExplorer(new EmptyModelMetadataProvider(), metadata, null)) };

            metadata.DisplayName.Returns("Test");

            helper.Process(null, output);

            Assert.Single(output.Attributes);
            Assert.Empty(output.Content.GetContent());
            Assert.Equal("Test", output.Attributes["placeholder"].Value);
        }

        #endregion
    }
}
