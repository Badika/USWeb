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
    public class EqualToAdapterTests
    {
        private EqualToAdapter adapter;
        private ClientModelValidationContext context;
        private Dictionary<String, String> attributes;

        public EqualToAdapterTests()
        {
            attributes = new Dictionary<String, String>();
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            adapter = new EqualToAdapter(new EqualToAttribute("AlternateStringField"));
            ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), "StringField");
            context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
        }

        #region AddValidation(ClientModelValidationContext context)

        [Fact]
        public void AddValidation_EqualTo()
        {
            adapter.AddValidation(context);

            Assert.Equal(3, attributes.Count);
            Assert.Equal("true", attributes["data-val"]);
            Assert.Equal("*." + adapter.Attribute.OtherPropertyName, attributes["data-val-equalto-other"]);
            Assert.Equal(Validation.For("EqualTo", context.ModelMetadata.PropertyName, adapter.Attribute.OtherPropertyName), attributes["data-val-equalto"]);
        }

        #endregion

        #region GetErrorMessage(ModelValidationContextBase context)

        [Fact]
        public void GetErrorMessage_EqualTo()
        {
            String expected = Validation.For("EqualTo", context.ModelMetadata.PropertyName, "");
            String actual = adapter.GetErrorMessage(context);

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
