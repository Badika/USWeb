using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using UpsCoolWeb.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace UpsCoolWeb.Components.Mvc
{
    public class StringLengthAdapter : StringLengthAttributeAdapter
    {
        public StringLengthAdapter(StringLengthAttribute attribute)
            : base(attribute, null)
        {
        }

        public override String GetErrorMessage(ModelValidationContextBase context)
        {
            Attribute.ErrorMessage = Validation.For(Attribute.MinimumLength == 0 ? "StringLength" : "StringLengthRange");

            return GetErrorMessage(context.ModelMetadata);
        }
    }
}
