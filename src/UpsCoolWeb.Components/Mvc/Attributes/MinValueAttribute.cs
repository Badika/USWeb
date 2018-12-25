using UpsCoolWeb.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace UpsCoolWeb.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class MinValueAttribute : ValidationAttribute
    {
        public Decimal Minimum { get; }

        public MinValueAttribute(Double minimum)
            : base(() => Validation.For("MinValue"))
        {
            Minimum = Convert.ToDecimal(minimum);
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, Minimum);
        }
        public override Boolean IsValid(Object value)
        {
            try
            {
                return value == null || Convert.ToDecimal(value) >= Minimum;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
