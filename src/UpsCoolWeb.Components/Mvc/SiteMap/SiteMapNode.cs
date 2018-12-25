using System;
using System.Collections.Generic;

namespace UpsCoolWeb.Components.Mvc
{
    public class SiteMapNode
    {
        public Boolean IsMenu { get; set; }
        public String IconClass { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean HasActiveChildren { get; set; }

        public String Controller { get; set; }
        public String Action { get; set; }
        public String Area { get; set; }

        public SiteMapNode Parent { get; set; }
        public IEnumerable<SiteMapNode> Children { get; set; }
    }
}
