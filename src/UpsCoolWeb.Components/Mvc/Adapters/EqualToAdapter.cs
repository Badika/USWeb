using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using UpsCoolWeb.Resources;
using System;

namespace UpsCoolWeb.Components.Mvc
{
    public class EqualToAdapter : AttributeAdapterBase<EqualToAttribute>
    {
        public EqualToAdapter(EqualToAttribute attribute)
            : base(attribute, null)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            Attribute.OtherPropertyDisplayName = Resource.ForProperty(context.ModelMetadata.ContainerType, Attribute.OtherPropertyName);
            Attribute.OtherPropertyDisplayName = Attribute.OtherPropertyDisplayName ?? Attribute.OtherPropertyName;

            context.Attributes["data-val-equalto-other"] = "*." + Attribute.OtherPropertyName;
            context.Attributes["data-val-equalto"] = GetErrorMessage(context);
            context.Attributes["data-val"] = "true";
        }
        public override String GetErrorMessage(ModelValidationContextBase context)
        {
            return GetErrorMessage(context.ModelMetadata);
        }
    }
}
