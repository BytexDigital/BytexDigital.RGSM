using System;
using System.Linq.Expressions;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Domain.Entities.Arma3;
using BytexDigital.RGSM.Node.Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Persistence
{
    public class NodeDbContext : DbContext
    {
        public NodeDbContext(DbContextOptions<NodeDbContext> options) : base(options)
        {
        }

        public DbSet<Setting> NodeSettings { get; set; }
        public DbSet<Arma3Server> Arma3Server { get; set; }

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
