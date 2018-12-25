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
    public class AcceptFilesAdapterTests
    {
        private AcceptFilesAdapter adapter;
        private ClientModelValidationContext context;
        private Dictionary<String, String> attributes;

        public AcceptFilesAdapterTests()
        {
            attributes = new Dictionary<String, String>();
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            adapter = new AcceptFilesAdapter(new AcceptFilesAttribute(".docx,.rtf"));
            ModelMetadata metadata = provider.GetMetadataForProperty(typeof(AllTypesView), "FileField");
            context = new ClientModelValidationContext(new ActionContext(), metadata, provider, attributes);
        }

        #region AddValidation(ClientModelValidationContext context)

        [Fact]
        public void AddValidation_AcceptFiles()
        {
            adapter.AddValidation(context);

            Assert.Equal(3, attributes.Count);
            Assert.Equal("true", attributes["data-val"]);
            Assert.Equal(".docx,.rtf", attributes["data-val-acceptfiles-extensions"]);
            Assert.Equal(Validation.For("AcceptFiles", context.ModelMetadata.PropertyName, ".docx,.rtf"), attributes["data-val-acceptfiles"]);
        }

        #endregion

        #region GetErrorMessage(ModelValidationContextBase context)

        [Fact]
        public void GetErrorMessage_AcceptFiles()
        {
            String expected = Validation.For("AcceptFiles", context.ModelMetadata.PropertyName, ".docx,.rtf");
            String actual = adapter.GetErrorMessage(context);

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
