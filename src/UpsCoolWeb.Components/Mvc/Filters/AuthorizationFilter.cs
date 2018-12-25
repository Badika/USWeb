using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using UpsCoolWeb.Components.Extensions;
using UpsCoolWeb.Components.Security;
using System;

namespace UpsCoolWeb.Components.Mvc
{
    public class AuthorizationFilter : IResourceFilter
    {
        private IAuthorization Authorization { get; }

        public AuthorizationFilter(IAuthorization authorization)
        {
            Authorization = authorization;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
                return;

            Int32? accountId = context.HttpContext.User.Id();
            String area = context.RouteData.Values["area"] as String;
            String action = context.RouteData.Values["action"] as String;
            String controller = context.RouteData.Values["controller"] as String;

            if (Authorization?.IsGrantedFor(accountId, area, controller, action) == false)
                context.Result = RedirectToNotFound(context);
        }
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        private IActionResult RedirectToNotFound(ActionContext context)
        {
            RouteValueDictionary route = new RouteValueDictionary();
            route["language"] = context.RouteData.Values["language"];
            route["action"] = "NotFound";
            route["controller"] = "Home";
            route["area"] = "";

            return new RedirectToRouteResult(route);
        }
    }
}
