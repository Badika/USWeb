using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class NumberValidatorProviderTests
    {
        #region CreateValidators(ClientValidatorProviderContext context)

        [Theory]
        [InlineData(typeof(Byte))]
        [InlineData(typeof(Byte?))]
        [InlineData(typeof(SByte))]
        [InlineData(typeof(SByte?))]
        [InlineData(typeof(Int16))]
        [InlineData(typeof(Int16?))]
        [InlineData(typeof(UInt16))]
        [InlineData(typeof(UInt16?))]
        [InlineData(typeof(Int32))]
        [InlineData(typeof(Int32?))]
        [InlineData(typeof(UInt32))]
        [InlineData(typeof(UInt32?))]
        [InlineData(typeof(Int64))]
        [InlineData(typeof(Int64?))]
        [InlineData(typeof(UInt64))]
        [InlineData(typeof(UInt64?))]
        [InlineData(typeof(Single))]
        [InlineData(typeof(Single?))]
        [InlineData(typeof(Double))]
        [InlineData(typeof(Double?))]
        [InlineData(typeof(Decimal))]
        [InlineData(typeof(Decimal?))]
        public void CreateValidators_ForNumber(Type type)
        {
            ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(type);
            ClientValidatorProviderContext context = new ClientValidatorProviderContext(metadata, new List<ClientValidatorItem>());

            new NumberValidatorProvider().CreateValidators(context);

            ClientValidatorItem actual = context.Results.Single();

            Assert.IsType<NumberValidator>(actual.Validator);
            Assert.Null(actual.ValidatorMetadata);
            Assert.True(actual.IsReusable);
        }

        [Fact]
        public void CreateValidators_DoesNotCreate()
        {
            ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(String));
            ClientValidatorProviderContext context = new ClientValidatorProviderContext(metadata, new List<ClientValidatorItem>());

            new NumberValidatorProvider().CreateValidators(context);

            Assert.Empty(context.Results);
        }

        #endregion
    }
}
