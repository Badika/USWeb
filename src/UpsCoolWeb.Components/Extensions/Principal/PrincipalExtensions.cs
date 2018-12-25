using System;
using System.Security.Principal;

namespace UpsCoolWeb.Components.Extensions
{
    public static class PrincipalExtensions
    {
        public static Int32? Id(this IPrincipal principal)
        {
            String id = principal.Identity.Name;
            if (String.IsNullOrEmpty(id))
                return null;

            return Int32.Parse(id);
        }
    }
}
