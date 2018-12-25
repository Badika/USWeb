using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Globalization;

namespace UpsCoolWeb.Components.Mvc
{
    public class MinValueAdapter : AttributeAdapterBase<MinValueAttribute>
    {
        public MinValueAdapter(MinValueAttribute attribute)
            : base(attribute, null)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-range"] = GetErrorMessage(context);
            context.Attributes["data-val-range-min"] = Attribute.Minimum.ToString(CultureInfo.InvariantCulture);
        }
        public override String GetErrorMessage(ModelValidationContextBase context)
        {
            return GetErrorMessage(context.ModelMetadata);
        }
    }
}
