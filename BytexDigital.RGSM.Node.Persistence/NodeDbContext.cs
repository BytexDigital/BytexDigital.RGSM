using System;
using System.Linq.Expressions;

using BytexDigital.RGSM.Domain.Interfaces;
using BytexDigital.RGSM.Node.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Persistence
{
    public class NodeDbContext : DbContext
    {
        public NodeDbContext(
            DbContextOptions<NodeDbContext> options) : base(options)
        {
        }

        public DbSet<NodeSetting> NodeSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Ensure IdentityServer adds its settings

            builder.ApplyConfigurationsFromAssembly(typeof(NodeDbContext).Assembly);
        }

        public T CreateEntity<T>(Expression<Func<NodeDbContext, DbSet<T>>> dbSetAccessor, bool attach = true) where T : class, new()
        {
            T model = null;

            if (ChangeTracker.LazyLoadingEnabled)
            {
                model = dbSetAccessor.Compile().Invoke(this).CreateProxy();
            }
            else
            {
                model = Activator.CreateInstance(typeof(T)) as T;
            }

            if (model is IHasCreationTimestamp modelWithCreationTimestamp)
            {
                modelWithCreationTimestamp.TimeCreated = DateTimeOffset.UtcNow;
            }

            if (attach)
            {
                Entry(model).State = EntityState.Added;
            }

            return model;
        }
    }
}
