using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace UpsCoolWeb.Resources
{
    public static class Resource
    {
        private static ConcurrentDictionary<String, ResourceSet> Resources { get; }

        static Resource()
        {
            Resources = new ConcurrentDictionary<String, ResourceSet>();
            String path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Resources";

            foreach (String resource in Directory.GetFiles(path, "*.json", SearchOption.AllDirectories))
            {
                String type = Path.GetFileNameWithoutExtension(resource);
                String language = Path.GetExtension(type).TrimStart('.');
                type = Path.GetFileNameWithoutExtension(type);

                if (!Resources.ContainsKey(type))
                    Resources[type] = new ResourceSet();

                Resources[type].Add(language, File.ReadAllText(resource));
            }
        }

        public static ResourceSet Set(String type)
        {
            if (!Resources.ContainsKey(type))
                Resources[type] = new ResourceSet();

            return Resources[type];
        }

        public static String ForAction(String name)
        {
            return Localized("Shared", "Actions", name);
        }

        public static String ForLookup(String type)
        {
            return Localized("Lookup", "Titles", type);
        }

        public static String ForString(String value)
        {
            return Localized("Shared", "Strings", value);
        }

        public static String ForPage(String header)
        {
            return Localized("Page", "Headers", header);
        }
        public static String ForPage(IDictionary<String, Object> values)
        {
            String area = values["area"] as String;
            String action = values["action"] as String;
            String controller = values["controller"] as String;

            return Localized("Page", "Titles", area + controller + action);
        }

        public static String ForSiteMap(String area, String controller, String action)
        {
            return Localized("SiteMap", "Titles", area + controller + action);
        }

        public static String ForPermission(String area)
        {
            return Localized("Permission", "Areas", area ?? "");
        }
        public static String ForPermission(String area, String controller)
        {
            return Localized("Permission", "Controllers", area + controller);
        }
        public static String ForPermission(String area, String controller, String action)
        {
            return Localized("Permission", "Actions", area + controller + action);
        }

        public static String ForProperty<TView, TProperty>(Expression<Func<TView, TProperty>> expression)
        {
            return ForProperty(expression.Body);
        }
        public static String ForProperty(String view, String name)
        {
            if (Localized(view, "Titles", name) is String title)
                return title;

            String[] properties = SplitCamelCase(name);
            String language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            for (Int32 skipped = 0; skipped < properties.Length; skipped++)
            {
                for (Int32 viewSize = 1; viewSize < properties.Length - skipped; viewSize++)
                {
                    String relation = String.Concat(properties.Skip(skipped).Take(viewSize)) + "View";
                    String property = String.Concat(properties.Skip(viewSize + skipped));

                    if (Localized(relation, "Titles", property) is String relationTitle)
                    {
                        if (!Resources.ContainsKey(view))
                            Resources[view] = new ResourceSet();

                        Resources[view][language, "Titles", name] = relationTitle;

                        return relationTitle;
                    }
                }
            }

            return null;
        }
        public static String ForProperty(Type view, String name)
        {
            return ForProperty(view.Name, name ?? "");
        }
        public static String ForProperty(Expression expression)
        {
            return expression is MemberExpression member ? ForProperty(member.Expression.Type, member.Member.Name) : null;
        }

        internal static String Localized(String type, String group, String key)
        {
            ResourceSet resources = Set(type);
            String language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            return resources[language, group, key] ?? resources["", group, key];
        }

        private static String[] SplitCamelCase(String value)
        {
            return Regex.Split(value, "(?<!^)(?=[A-Z])");
        }
    }
}
