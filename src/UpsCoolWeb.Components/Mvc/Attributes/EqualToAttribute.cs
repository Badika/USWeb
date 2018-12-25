using UpsCoolWeb.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UpsCoolWeb.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class EqualToAttribute : ValidationAttribute
    {
        public String OtherPropertyName { get; }
        public String OtherPropertyDisplayName { get; set; }

        public EqualToAttribute(String otherPropertyName)
            : base(() => Validation.For("EqualTo"))
        {
            OtherPropertyName = otherPropertyName ?? throw new ArgumentNullException(nameof(otherPropertyName));
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, OtherPropertyDisplayName);
        }
        protected override ValidationResult IsValid(Object value, ValidationContext context)
        {
            PropertyInfo other = context.ObjectType.GetProperty(OtherPropertyName);
            if (other != null && Equals(value, other.GetValue(context.ObjectInstance)))
                return null;

            OtherPropertyDisplayName = Resource.ForProperty(context.ObjectType, OtherPropertyName) ?? OtherPropertyName;

            return new ValidationResult(FormatErrorMessage(context.DisplayName));
        }
    }
}
