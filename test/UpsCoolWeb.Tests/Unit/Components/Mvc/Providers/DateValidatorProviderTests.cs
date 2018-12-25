using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class DateValidatorProviderTests
    {
        #region CreateValidators(ClientValidatorProviderContext context)

        [Theory]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(DateTime?))]
        public void CreateValidators_ForDate(Type type)
        {
            ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(type);
            ClientValidatorProviderContext context = new ClientValidatorProviderContext(metadata, new List<ClientValidatorItem>());

            new DateValidatorProvider().CreateValidators(context);

            ClientValidatorItem actual = context.Results.Single();

            Assert.IsType<DateValidator>(actual.Validator);
            Assert.Null(actual.ValidatorMetadata);
            Assert.True(actual.IsReusable);
        }

        [Fact]
        public void CreateValidators_DoesNotCreate()
        {
            ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(String));
            ClientValidatorProviderContext context = new ClientValidatorProviderContext(metadata, new List<ClientValidatorItem>());

            new DateValidatorProvider().CreateValidators(context);

            Assert.Empty(context.Results);
        }

        #endregion
    }
}
