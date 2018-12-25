using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using UpsCoolWeb.Resources;

namespace UpsCoolWeb.Components.Mvc
{
    public class DisplayMetadataProvider : IDisplayMetadataProvider
    {
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context.Key.ContainerType != null && context.Key.MetadataKind == ModelMetadataKind.Property)
                context.DisplayMetadata.DisplayName = () => Resource.ForProperty(context.Key.ContainerType, context.Key.Name);
        }
    }
}
