using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;

namespace UpsCoolWeb.Data.Logging
{
    public class LoggableProperty
    {
        private Object OldValue { get; }
        private Object NewValue { get; }
        private String Property { get; }
        public Boolean IsModified { get; }

        public LoggableProperty(PropertyEntry entry, Object newValue)
        {
            NewValue = newValue;
            OldValue = entry.CurrentValue;
            Property = entry.Metadata.Name;
            IsModified = entry.IsModified && !Equals(NewValue, OldValue);
        }

        public override String ToString()
        {
            if (IsModified)
                return Property + ": " + Format(NewValue) + " => " + Format(OldValue);

            return Property + ": " + Format(NewValue);
        }

        private String Format(Object value)
        {
            if (value is null)
                return "null";

            if (value is DateTime date)
                return "\"" + date.ToString("yyyy-MM-dd HH:mm:ss") + "\"";

            return JsonConvert.ToString(value);
        }
    }
}
