using Microsoft.AspNetCore.Mvc.ModelBinding;
using NonFactors.Mvc.Lookup;
using NSubstitute;
using System;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class TrimmingModelBinderProviderTests
    {
        #region GetBinder(ModelBinderProviderContext context)

        [Fact]
        public void GetBinder_ForLookupFilterReturnsNull()
        {
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelBinderProviderContext context = Substitute.For<ModelBinderProviderContext>();

            context.Metadata.Returns(provider.GetMetadataForProperty(typeof(LookupFilter), "Search"));

            Assert.Null(new TrimmingModelBinderProvider().GetBinder(context));
        }

        [Fact]
        public void GetBinder_ForString()
        {
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelBinderProviderContext context = Substitute.For<ModelBinderProviderContext>();

            context.Metadata.Returns(provider.GetMetadataForType(typeof(String)));

            Assert.IsType<TrimmingModelBinder>(new TrimmingModelBinderProvider().GetBinder(context));
        }

        [Fact]
        public void GetBinder_ForNotStringReturnsNull()
        {
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelBinderProviderContext context = Substitute.For<ModelBinderProviderContext>();

            context.Metadata.Returns(provider.GetMetadataForType(typeof(DateTime)));

            Assert.Null(new TrimmingModelBinderProvider().GetBinder(context));
        }

        #endregion
    }
}
