using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace UpsCoolWeb.Components.Security
{
    public class AuthenticationEvents : CookieAuthenticationEvents
    {
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            LinkGenerator link = context.HttpContext.RequestServices.GetService<LinkGenerator>();
            Object route = new { area = "", returnUrl = context.Request.PathBase + context.Request.Path };

            context.RedirectUri = link.GetPathByAction(context.HttpContext, "Login", "Auth", route);

            return base.RedirectToLogin(context);
        }
    }
}
