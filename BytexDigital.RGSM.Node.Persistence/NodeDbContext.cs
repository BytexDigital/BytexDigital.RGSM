using System;
using System.Diagnostics;
using System.Linq.Expressions;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Domain.Entities.Arma3;
using BytexDigital.RGSM.Node.Domain.Entities.Scheduling;
using BytexDigital.RGSM.Node.Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Persistence
{
    public class NodeDbContext : DbContext, IDisposable
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public NodeDbContext(DbContextOptions<NodeDbContext> options) : base(options)
        {
            Debug.WriteLine($"NodeDbContext created: {ContextId}");
        }

        public override void Dispose()
        {
            Debug.WriteLine($"DISPOSE {Id}");

            base.Dispose();
        }

        public DbSet<KeyValue> KeyValues { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<SchedulerPlan> SchedulerPlans { get; set; }
        public DbSet<ScheduleGroup> ScheduleGroups { get; set; }
        public DbSet<ScheduleAction> ScheduleActions { get; set; }
        public DbSet<Arma3Server> Arma3Server { get; set; }
        public DbSet<TrackedDepot> TrackedDepots { get; set; }
        public DbSet<TrackedWorkshopMod> TrackedWorkshopMods { get; set; }
        public DbSet<GroupReference> GroupReferences { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }

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
