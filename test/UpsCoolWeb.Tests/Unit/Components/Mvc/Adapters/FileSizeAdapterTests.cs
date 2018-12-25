using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using UpsCoolWeb.Resources;
using UpsCoolWeb.Tests;
using System;
using System.Collections.Generic;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class FileSizeAdapterTests
    {
        private FileSizeAdapter adapter;
        private ClientModelValidationContext context;
        private Dictionary<String, String> attributes;

        public FileSizeAdapterTests()
        {
            attributes = new Dictionary<String, String>();
            adapter = new FileSizeAdapter(new FileSizeAttribute(12.25));
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), "FileField");
            context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
        }

        #region AddValidation(ClientModelValidationContext context)

        [Fact]
        public void AddValidation_FileSize()
        {
            adapter.AddValidation(context);

            Assert.Equal(3, attributes.Count);
            Assert.Equal("true", attributes["data-val"]);
            Assert.Equal("12845056.00", attributes["data-val-filesize-max"]);
            Assert.Equal(Validation.For("FileSize", context.ModelMetadata.PropertyName, 12.25), attributes["data-val-filesize"]);
        }

        #endregion

        #region GetErrorMessage(ModelValidationContextBase context)

        [Fact]
        public void GetErrorMessage_FileSize()
        {
            String expected = Validation.For("FileSize", context.ModelMetadata.PropertyName, 12.25);
            String actual = adapter.GetErrorMessage(context);

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
