using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using System;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class AjaxOnlyAttributeTests
    {
        #region IsValidForRequest(RouteContext context, ActionDescriptor action)

        [Theory]
        [InlineData("", false)]
        [InlineData("XMLHttpRequest", true)]
        public void IsValidForRequest_Ajax(String header, Boolean isValid)
        {
            RouteContext context = new RouteContext(Substitute.For<HttpContext>());
            context.HttpContext.Request.Headers["X-Requested-With"].Returns(new StringValues(header));

            Boolean actual = new AjaxOnlyAttribute().IsValidForRequest(context, null);
            Boolean expected = isValid;

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
