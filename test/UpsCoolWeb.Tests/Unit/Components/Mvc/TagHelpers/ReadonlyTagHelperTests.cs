using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;
using System;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class ReadOnlyTagHelperTests
    {
        #region Process(TagHelperContext context, TagHelperOutput output)

        [Fact]
        public void Process_ReadOnly()
        {
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
            TagHelperOutput output = new TagHelperOutput("input", new TagHelperAttributeList(), (useCache, encoder) => null);
            ReadOnlyTagHelper helper = new ReadOnlyTagHelper { For = new ModelExpression("Total", new ModelExplorer(new EmptyModelMetadataProvider(), metadata, null)) };

            metadata.IsReadOnly.Returns(true);

            helper.Process(null, output);

            Assert.Single(output.Attributes);
            Assert.Empty(output.Content.GetContent());
            Assert.Equal("readonly", output.Attributes["readonly"].Value);
        }

        [Fact]
        public void Process_Editable()
        {
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
            TagHelperOutput output = new TagHelperOutput("input", new TagHelperAttributeList(), (useCache, encoder) => null);
            ReadOnlyTagHelper helper = new ReadOnlyTagHelper { For = new ModelExpression("Total", new ModelExplorer(new EmptyModelMetadataProvider(), metadata, null)) };

            metadata.IsReadOnly.Returns(false);

            helper.Process(null, output);

            Assert.Empty(output.Attributes);
            Assert.Empty(output.Content.GetContent());
        }

        #endregion
    }
}
