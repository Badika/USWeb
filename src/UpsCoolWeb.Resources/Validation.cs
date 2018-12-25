using System;

namespace UpsCoolWeb.Resources
{
    public static class Validation
    {
        public static String For(String key, params Object[] args)
        {
            String validation = Resource.Localized("Form", "Validations", key);

            return validation == null || args.Length == 0 ? validation : String.Format(validation, args);
        }
        public static String For<TView>(String key, params Object[] args)
        {
            String validation = Resource.Localized(typeof(TView).Name, "Validations", key);

            return validation == null || args.Length == 0 ? validation : String.Format(validation, args);
        }
    }
}
