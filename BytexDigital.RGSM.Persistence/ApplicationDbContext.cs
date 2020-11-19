using System;
using System.Linq.Expressions;

using BytexDigital.RGSM.Domain.Entities;
using BytexDigital.RGSM.Domain.Games.Arma3.Entities;
using BytexDigital.RGSM.Domain.Interfaces;

using IdentityServer4.EntityFramework.Options;

using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BytexDigital.RGSM.Persistence
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<ApplicationUserGroup> ApplicationUserGroups { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<GroupPermission> GroupPermissions { get; set; }
        public DbSet<Arma3Server> Arma3Servers { get; set; }
        public DbSet<SharedSetting> SharedSettings { get; set; }
        public DbSet<SteamCredential> SteamCredentials { get; set; }
        public DbSet<SteamCredentialSupportedApp> SteamCredentialSupportedApps { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Ensure IdentityServer adds its settings

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public T CreateEntity<T>(Expression<Func<ApplicationDbContext, DbSet<T>>> dbSetAccessor, bool attach = true) where T : class, new()
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
