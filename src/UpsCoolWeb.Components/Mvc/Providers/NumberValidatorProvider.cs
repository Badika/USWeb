using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace UpsCoolWeb.Components.Mvc
{
    public class NumberValidatorProvider : IClientModelValidatorProvider
    {
        private HashSet<Type> NumericTypes { get; }

        public NumberValidatorProvider()
        {
            NumericTypes = new HashSet<Type>
            {
              typeof(Byte),
              typeof(SByte),
              typeof(Int16),
              typeof(UInt16),
              typeof(Int32),
              typeof(UInt32),
              typeof(Int64),
              typeof(UInt64),
              typeof(Single),
              typeof(Double),
              typeof(Decimal)
            };
        }

        public void CreateValidators(ClientValidatorProviderContext context)
        {
            Type type = Nullable.GetUnderlyingType(context.ModelMetadata.ModelType) ?? context.ModelMetadata.ModelType;

            if (NumericTypes.Contains(type))
                context.Results.Add(new ClientValidatorItem
                {
                    Validator = new NumberValidator(),
                    IsReusable = true
                });
        }
    }
}
