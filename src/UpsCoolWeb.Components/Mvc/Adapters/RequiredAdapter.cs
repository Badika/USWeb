using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using UpsCoolWeb.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace UpsCoolWeb.Components.Mvc
{
    public class RequiredAdapter : RequiredAttributeAdapter
    {
        public RequiredAdapter(RequiredAttribute attribute)
            : base(attribute, null)
        {
        }

        public override String GetErrorMessage(ModelValidationContextBase context)
        {
            Attribute.ErrorMessage = Validation.For("Required");

            return GetErrorMessage(context.ModelMetadata);
        }
    }
}
