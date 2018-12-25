using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using UpsCoolWeb.Tests;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class TrimmingModelBinderTests
    {
        private TrimmingModelBinder binder;
        private ModelBindingContext context;

        public TrimmingModelBinderTests()
        {
            binder = new TrimmingModelBinder();
            context = new DefaultModelBindingContext();
            context.ModelState = new ModelStateDictionary();
            context.ValueProvider = Substitute.For<IValueProvider>();
        }

        #region BindModelAsync(ModelBindingContext context)

        [Fact]
        public async Task BindModelAsync_NoValue()
        {
            ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(String));
            context.ValueProvider.GetValue(context.ModelName).Returns(ValueProviderResult.None);
            context.ModelMetadata = metadata;

            await binder.BindModelAsync(context);

            ModelBindingResult expected = new ModelBindingResult();
            ModelBindingResult actual = context.Result;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public async Task BindModelAsync_Null(String value)
        {
            ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForProperty(typeof(AllTypesView), "StringField");
            context.ValueProvider.GetValue("StringField").Returns(new ValueProviderResult(value));
            context.ModelName = "StringField";
            context.ModelMetadata = metadata;

            await binder.BindModelAsync(context);

            ModelBindingResult expected = ModelBindingResult.Success(null);
            ModelBindingResult actual = context.Result;

            Assert.Equal(expected.IsModelSet, actual.IsModelSet);
            Assert.Equal(expected.Model, actual.Model);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public async Task BindModelAsync_Empty(String value)
        {
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
            context.ValueProvider.GetValue("StringField").Returns(new ValueProviderResult(value));
            metadata.ConvertEmptyStringToNull.Returns(false);
            context.ModelName = "StringField";
            context.ModelMetadata = metadata;

            await binder.BindModelAsync(context);

            ModelBindingResult expected = ModelBindingResult.Success("");
            ModelBindingResult actual = context.Result;

            Assert.Equal(expected.IsModelSet, actual.IsModelSet);
            Assert.Equal(expected.Model, actual.Model);
        }

        [Fact]
        public async Task BindModelAsync_Trimmed()
        {
            ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForProperty(typeof(AllTypesView), "StringField");
            context.ValueProvider.GetValue("StringField").Returns(new ValueProviderResult(" Value "));
            context.ModelName = "StringField";
            context.ModelMetadata = metadata;

            await binder.BindModelAsync(context);

            ModelBindingResult expected = ModelBindingResult.Success("Value");
            ModelBindingResult actual = context.Result;

            Assert.Equal(expected.IsModelSet, actual.IsModelSet);
            Assert.Equal(expected.Model, actual.Model);
        }

        #endregion
    }
}
