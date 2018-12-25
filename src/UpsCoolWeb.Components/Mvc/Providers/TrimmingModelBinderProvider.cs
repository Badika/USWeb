using Microsoft.AspNetCore.Mvc.ModelBinding;
using NonFactors.Mvc.Lookup;
using System;

namespace UpsCoolWeb.Components.Mvc
{
    public class TrimmingModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ContainerType == typeof(LookupFilter))
                return null;

            return context.Metadata.ModelType == typeof(String) ? new TrimmingModelBinder() : null;
        }
    }
}
