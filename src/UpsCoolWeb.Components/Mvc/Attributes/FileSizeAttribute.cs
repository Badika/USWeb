using Microsoft.AspNetCore.Http;
using UpsCoolWeb.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace UpsCoolWeb.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class FileSizeAttribute : ValidationAttribute
    {
        public Decimal MaximumMB { get; }

        public FileSizeAttribute(Double maximumMB)
            : base(() => Validation.For("FileSize"))
        {
            MaximumMB = Convert.ToDecimal(maximumMB);
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, MaximumMB);
        }
        public override Boolean IsValid(Object value)
        {
            IEnumerable<IFormFile> files = ToFiles(value);

            return files == null || files.Sum(file => file?.Length ?? 0) <= MaximumMB * 1024 * 1024;
        }

        private IEnumerable<IFormFile> ToFiles(Object value)
        {
            return value is IFormFile file ? new[] { file } : value as IEnumerable<IFormFile>;
        }
    }
}
