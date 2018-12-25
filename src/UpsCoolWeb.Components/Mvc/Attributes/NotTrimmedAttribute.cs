using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace UpsCoolWeb.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class NotTrimmedAttribute : ModelBinderAttribute, IModelBinder
    {
        public NotTrimmedAttribute()
        {
            BinderType = GetType();
        }

        public async Task BindModelAsync(ModelBindingContext context)
        {
            ILoggerFactory logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();

            await new SimpleTypeModelBinder(typeof(String), logger).BindModelAsync(context);
        }
    }
}
