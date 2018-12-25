using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace UpsCoolWeb.Components.Mvc
{
    public class Languages : ILanguages
    {
        public Language Default
        {
            get;
        }
        public Language Current
        {
            get
            {
                return Supported.Single(language => language.Culture.Equals(CultureInfo.CurrentUICulture));
            }
            set
            {
                Thread.CurrentThread.CurrentCulture = value.Culture;
                Thread.CurrentThread.CurrentUICulture = value.Culture;
            }
        }
        public Language[] Supported
        {
            get;
        }

        private Dictionary<String, Language> Dictionary
        {
            get;
        }

        public Languages(String defaultLanguage, Language[] supported)
        {
            Dictionary = supported.ToDictionary(language => language.Abbreviation);
            Default = Dictionary[defaultLanguage];
            Supported = supported;
        }

        public Language this[String abbreviation] => Dictionary.TryGetValue(abbreviation ?? "", out Language language) ? language : Default;
    }
}
