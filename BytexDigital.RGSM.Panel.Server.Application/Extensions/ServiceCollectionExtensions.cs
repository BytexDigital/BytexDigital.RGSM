using BytexDigital.RGSM.Panel.Server.Application.Core;

using Microsoft.Extensions.DependencyInjection;

namespace BytexDigital.RGSM.Panel.Server.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services
                .AddScoped<DatabaseDefaultsService>()
                .AddScoped<AccountService>()
                .AddScoped<SettingService>()
                .AddScoped<SteamCredentialService>()
                .AddScoped<AuthenticationService>()
                .AddScoped<NodeService>()
                .AddScoped<GroupService>();

            return services;
        }
    }
}
