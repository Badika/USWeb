using System.Diagnostics.CodeAnalysis;

namespace UpsCoolWeb.Components.Security.Tests
{
    [AllowUnauthorized]
    [ExcludeFromCodeCoverage]
    public class AllowUnauthorizedController : AuthorizedController
    {
    }
}
