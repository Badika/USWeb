using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace UpsCoolWeb.Resources
{
    internal class ResourceDictionary : ConcurrentDictionary<String, String>
    {
        public ResourceDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
