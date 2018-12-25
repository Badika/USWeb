using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class SecureHeadersMiddlewareTests
    {
        #region Invoke(HttpContext context)

        [Fact]
        public async Task Invoke_AddsSecureHeaders()
        {
            HttpContext context = new DefaultHttpContext();

            await new SecureHeadersMiddleware(next => Task.CompletedTask).Invoke(context);

            IHeaderDictionary actual = context.Response.Headers;

            Assert.Equal("script-src 'self'; style-src 'self'; object-src 'none'", actual["Content-Security-Policy"]);
            Assert.Equal("1; mode=block", actual["X-XSS-Protection"]);
            Assert.Equal("nosniff", actual["X-Content-Type-Options"]);
            Assert.Equal("deny", actual["X-Frame-Options"]);
            Assert.Equal(4, actual.Keys.Count);
        }

        #endregion
    }
}
