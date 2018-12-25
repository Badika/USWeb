using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UpsCoolWeb.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UpsCoolWeb.Data.Logging
{
    public class AuditLogger : IAuditLogger
    {
        private Int32? AccountId { get; }
        private DbContext Context { get; }
        private List<LoggableEntity> Entities { get; }

        public AuditLogger(DbContext context, Int32? accountId)
        {
            Context = context;
            AccountId = accountId;
            Entities = new List<LoggableEntity>();
            Context.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public void Log(IEnumerable<EntityEntry<BaseModel>> entries)
        {
            foreach (EntityEntry<BaseModel> entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Deleted:
                    case EntityState.Modified:
                        LoggableEntity entity = new LoggableEntity(entry);
                        if (entity.Properties.Any())
                            Log(entity);
                        break;
                }
            }
        }
        public void Log(LoggableEntity entity)
        {
            Entities.Add(entity);
        }
        public void Save()
        {
            foreach (LoggableEntity entity in Entities)
            {
                AuditLog log = new AuditLog();
                log.Changes = entity.ToString();
                log.EntityName = entity.Name;
                log.Action = entity.Action;
                log.EntityId = entity.Id();
                log.AccountId = AccountId;

                Context.Add(log);
            }

            Context.SaveChanges();
            Entities.Clear();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
