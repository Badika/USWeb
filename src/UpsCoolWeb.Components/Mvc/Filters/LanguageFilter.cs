using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace UpsCoolWeb.Components.Mvc
{
    public class LanguageFilter : IResourceFilter
    {
        private ILanguages Languages { get; }

        public LanguageFilter(ILanguages languages)
        {
            Languages = languages;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            Languages.Current = Languages[context.RouteData.Values["language"] as String];
        }
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
