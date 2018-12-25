using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;
using System;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class FormInputTagHelperTests
    {
        #region Process(TagHelperContext context, TagHelperOutput output)

        [Fact]
        public void Process_Boolean()
        {
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(Boolean)));
            TagHelperOutput output = new TagHelperOutput("input", new TagHelperAttributeList(), (useCache, encoder) => null);
            FormInputTagHelper helper = new FormInputTagHelper { For = new ModelExpression("IsChecked", new ModelExplorer(new EmptyModelMetadataProvider(), metadata, null)) };

            helper.Process(null, output);

            Assert.Empty(output.Attributes);
            Assert.Empty(output.Content.GetContent());
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("on", "on")]
        [InlineData(null, null)]
        [InlineData("off", "off")]
        public void Process_Autocomplete(String value, String expectedValue)
        {
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
            TagHelperOutput output = new TagHelperOutput("input", new TagHelperAttributeList(), (useCache, encoder) => null);
            FormInputTagHelper helper = new FormInputTagHelper { For = new ModelExpression("IsChecked", new ModelExplorer(new EmptyModelMetadataProvider(), metadata, null)) };

            output.Attributes.Add("autocomplete", value);

            helper.Process(null, output);

            Assert.Equal(2, output.Attributes.Count);
            Assert.Empty(output.Content.GetContent());
            Assert.Equal("form-control", output.Attributes["class"].Value);
            Assert.Equal(expectedValue, output.Attributes["autocomplete"].Value);
        }

        [Theory]
        [InlineData("", "form-control ")]
        [InlineData(null, "form-control ")]
        [InlineData("test", "form-control test")]
        public void Process_Class(String value, String expectedValue)
        {
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
            TagHelperOutput output = new TagHelperOutput("input", new TagHelperAttributeList(), (useCache, encoder) => null);
            FormInputTagHelper helper = new FormInputTagHelper { For = new ModelExpression("IsChecked", new ModelExplorer(new EmptyModelMetadataProvider(), metadata, null)) };

            output.Attributes.Add("class", value);

            helper.Process(null, output);

            Assert.Equal(2, output.Attributes.Count);
            Assert.Empty(output.Content.GetContent());
            Assert.Equal("off", output.Attributes["autocomplete"].Value);
            Assert.Equal(expectedValue, output.Attributes["class"].Value);
        }

        [Fact]
        public void Process_Input()
        {
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
            TagHelperOutput output = new TagHelperOutput("input", new TagHelperAttributeList(), (useCache, encoder) => null);
            FormInputTagHelper helper = new FormInputTagHelper { For = new ModelExpression("IsChecked", new ModelExplorer(new EmptyModelMetadataProvider(), metadata, null)) };

            helper.Process(null, output);

            Assert.Equal(2, output.Attributes.Count);
            Assert.Empty(output.Content.GetContent());
            Assert.Equal("off", output.Attributes["autocomplete"].Value);
            Assert.Equal("form-control", output.Attributes["class"].Value);
        }

        #endregion
    }
}
