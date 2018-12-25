using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using UpsCoolWeb.Objects;
using UpsCoolWeb.Resources;
using System;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class DisplayMetadataProviderTests
    {
        #region CreateDisplayMetadata(DisplayMetadataProviderContext context)

        [Fact]
        public void CreateDisplayMetadata_SetsDisplayName()
        {
            DisplayMetadataProvider provider = new DisplayMetadataProvider();
            DisplayMetadataProviderContext context = new DisplayMetadataProviderContext(
                ModelMetadataIdentity.ForProperty(typeof(String), "Title", typeof(RoleView)),
                ModelAttributes.GetAttributesForType(typeof(RoleView)));

            provider.CreateDisplayMetadata(context);

            String expected = Resource.ForProperty(typeof(RoleView), "Title");
            String actual = context.DisplayMetadata.DisplayName();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateDisplayMetadata_NullContainerType_DoesNotSetDisplayName()
        {
            DisplayMetadataProvider provider = new DisplayMetadataProvider();
            DisplayMetadataProviderContext context = new DisplayMetadataProviderContext(
                   ModelMetadataIdentity.ForType(typeof(RoleView)),
                   ModelAttributes.GetAttributesForType(typeof(RoleView)));

            provider.CreateDisplayMetadata(context);

            Assert.Null(context.DisplayMetadata.DisplayName);
        }

        #endregion
    }
}
