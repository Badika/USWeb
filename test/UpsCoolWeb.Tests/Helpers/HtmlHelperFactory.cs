using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using UpsCoolWeb.Components.Security;
using NSubstitute;
using System;

namespace UpsCoolWeb.Tests
{
    public static class HtmlHelperFactory
    {
        public static IHtmlHelper CreateHtmlHelper()
        {
            return CreateHtmlHelper<Object>(null);
        }
        public static IHtmlHelper<T> CreateHtmlHelper<T>(T model)
        {
            IHtmlHelper<T> html = Substitute.For<IHtmlHelper<T>>();

            html.ViewContext.Returns(new ViewContext());
            html.ViewContext.RouteData = new RouteData();
            html.ViewContext.HttpContext = Substitute.For<HttpContext>();
            html.MetadataProvider.Returns(new EmptyModelMetadataProvider());
            html.ViewContext.HttpContext.RequestServices
                .GetService<IAuthorization>()
                .Returns(Substitute.For<IAuthorization>());
            html.ViewContext.ViewData.Model = model;

            return html;
        }
    }
}
