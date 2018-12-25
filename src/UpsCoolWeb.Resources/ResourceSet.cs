using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace UpsCoolWeb.Resources
{
    public class ResourceSet
    {
        private ConcurrentDictionary<String, ConcurrentDictionary<String, ResourceDictionary>> Source { get; }

        internal ResourceSet()
        {
            Source = new ConcurrentDictionary<String, ConcurrentDictionary<String, ResourceDictionary>>();
        }

        public String this[String language, String group, String key]
        {
            get
            {
                if (!Source.ContainsKey(language))
                    return null;

                if (!Source[language].ContainsKey(group))
                    return null;

                return Source[language][group].TryGetValue(key, out String title) ? title : null;
            }
            set
            {
                if (!Source.ContainsKey(language))
                    Source[language] = new ConcurrentDictionary<String, ResourceDictionary>();

                if (!Source[language].ContainsKey(group))
                    Source[language][group] = new ResourceDictionary();

                Source[language][group][key] = value;
            }
        }

        public void Inherit(ResourceSet resources)
        {
            foreach (String language in resources.Source.Keys)
            {
                if (Source.ContainsKey(language))
                {
                    foreach (String group in resources.Source[language].Keys)
                    {
                        if (Source[language].ContainsKey(group))
                        {
                            foreach (String key in resources.Source[language][group].Keys)
                                if (!Source[language][group].ContainsKey(key))
                                    Source[language][group][key] = resources.Source[language][group][key];
                        }
                        else
                        {
                            Source[language][group] = resources.Source[language][group];
                        }
                    }
                }
                else
                {
                    Source[language] = resources.Source[language];
                }
            }
        }

        public void Add(String language, String source)
        {
            Source[language] = JsonConvert.DeserializeObject<ConcurrentDictionary<String, ResourceDictionary>>(source);
        }
    }
}
