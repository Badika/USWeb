using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;

namespace UpsCoolWeb.Components.Mvc
{
    public class DateValidatorProvider : IClientModelValidatorProvider
    {
        public void CreateValidators(ClientValidatorProviderContext context)
        {
            if (typeof(DateTime?).IsAssignableFrom(context.ModelMetadata.UnderlyingOrModelType))
                context.Results.Add(new ClientValidatorItem
                {
                    Validator = new DateValidator(),
                    IsReusable = true
                });
        }
    }
}
