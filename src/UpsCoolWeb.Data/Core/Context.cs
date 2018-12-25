using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using UpsCoolWeb.Components.Mvc;
using UpsCoolWeb.Data.Mapping;
using UpsCoolWeb.Objects;
using System.Linq;
using System.Reflection;

namespace UpsCoolWeb.Data.Core
{
    public class Context : DbContext
    {
        #region Administration

        protected DbSet<Role> Role { get; set; }
        protected DbSet<Account> Account { get; set; }
        protected DbSet<Permission> Permission { get; set; }
        protected DbSet<RolePermission> RolePermission { get; set; }

        #endregion

        #region System

        protected DbSet<AuditLog> AuditLog { get; set; }

        #endregion

        static Context()
        {
            ObjectMapper.MapObjects();
        }
        protected Context()
        {
        }
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (IMutableEntityType entity in builder.Model.GetEntityTypes())
                foreach (PropertyInfo property in entity.ClrType.GetProperties())
                    if (property.GetCustomAttribute<IndexAttribute>(false) is IndexAttribute index)
                        builder.Entity(entity.ClrType).HasIndex(property.Name).IsUnique(index.IsUnique);

            builder.Entity<Permission>().Property(model => model.Id).ValueGeneratedNever();
            foreach (IMutableForeignKey key in builder.Model.GetEntityTypes().SelectMany(entity => entity.GetForeignKeys()))
                key.DeleteBehavior = DeleteBehavior.Restrict;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseLazyLoadingProxies();
        }
    }
}
