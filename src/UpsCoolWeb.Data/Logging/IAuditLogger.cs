using Microsoft.EntityFrameworkCore.ChangeTracking;
using UpsCoolWeb.Objects;
using System;
using System.Collections.Generic;

namespace UpsCoolWeb.Data.Logging
{
    public interface IAuditLogger : IDisposable
    {
        void Log(IEnumerable<EntityEntry<BaseModel>> entries);
        void Save();
    }
}
