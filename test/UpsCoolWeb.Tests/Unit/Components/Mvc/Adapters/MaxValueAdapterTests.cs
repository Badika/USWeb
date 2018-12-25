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
    public class MaxValueAdapterTests
    {
        private MaxValueAdapter adapter;
        private ClientModelValidationContext context;
        private Dictionary<String, String> attributes;

        public MaxValueAdapterTests()
        {
            attributes = new Dictionary<String, String>();
            adapter = new MaxValueAdapter(new MaxValueAttribute(128));
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), "Int32Field");
            context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
        }

        #region AddValidation(ClientModelValidationContext context)

        [Fact]
        public void AddValidation_MaxValue()
        {
            adapter.AddValidation(context);

            Assert.Equal(3, attributes.Count);
            Assert.Equal("true", attributes["data-val"]);
            Assert.Equal("128", attributes["data-val-range-max"]);
            Assert.Equal(Validation.For("MaxValue", context.ModelMetadata.PropertyName, 128), attributes["data-val-range"]);
        }

        #endregion

        #region GetErrorMessage(ModelValidationContextBase context)

        [Fact]
        public void GetErrorMessage_MaxValue()
        {
            String expected = Validation.For("MaxValue", context.ModelMetadata.PropertyName, 128);
            String actual = adapter.GetErrorMessage(context);

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
