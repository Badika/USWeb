using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace UpsCoolWeb.Components.Mvc
{
    public class SecureHeadersMiddleware
    {
        private RequestDelegate Next { get; }

        public SecureHeadersMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers["Content-Security-Policy"] = "script-src 'self'; style-src 'self'; object-src 'none'";
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["X-Frame-Options"] = "deny";

            await Next(context);
        }
    }
}
