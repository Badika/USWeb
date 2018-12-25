using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UpsCoolWeb.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace UpsCoolWeb.Data.Logging
{
    public class LoggableEntity
    {
        public String Name { get; }
        public String Action { get; }
        public Func<Int32> Id { get; }
        private static String IdName { get; }
        public IEnumerable<LoggableProperty> Properties { get; }

        static LoggableEntity()
        {
            IdName = typeof(BaseModel).GetProperties().Single(property => property.IsDefined(typeof(KeyAttribute), false)).Name;
        }

        public LoggableEntity(EntityEntry<BaseModel> entry)
        {
            PropertyValues values =
                entry.State == EntityState.Modified || entry.State == EntityState.Deleted
                    ? entry.GetDatabaseValues()
                    : entry.CurrentValues;

            Properties = values.Properties.Where(property => property.Name != IdName).Select(property => new LoggableProperty(entry.Property(property.Name), values[property]));
            Properties = entry.State == EntityState.Modified ? Properties.Where(property => property.IsModified) : Properties;
            Properties = Properties.ToArray();

            Name = entry.Entity.GetType().Name;
            Action = entry.State.ToString();
            Id = () => entry.Entity.Id;
        }

        public override String ToString()
        {
            StringBuilder changes = new StringBuilder();
            foreach (LoggableProperty property in Properties)
                changes.Append(property + Environment.NewLine);

            return changes.ToString();
        }
    }
}
