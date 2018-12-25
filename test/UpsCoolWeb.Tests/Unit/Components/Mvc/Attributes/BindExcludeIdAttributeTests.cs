using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using NSubstitute;
using System;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class BindExcludeIdAttributeTests
    {
        #region PropertyFilter

        [Theory]
        [InlineData("id", true)]
        [InlineData("iD", true)]
        [InlineData("ID", true)]
        [InlineData("Id", false)]
        [InlineData("Prop", true)]
        public void PropertyFilter_Id(String property, Boolean isIncluded)
        {
            ModelMetadataIdentity identity = ModelMetadataIdentity.ForProperty(typeof(Object), property, typeof(Object));
            ModelMetadata metadata = Substitute.ForPartsOf<ModelMetadata>(identity);

            Boolean actual = new BindExcludeIdAttribute().PropertyFilter(metadata);
            Boolean expected = isIncluded;

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
