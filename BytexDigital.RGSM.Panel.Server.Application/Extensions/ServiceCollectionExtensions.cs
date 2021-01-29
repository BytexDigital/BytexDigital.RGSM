using BytexDigital.RGSM.Panel.Server.Application.Core;
using BytexDigital.RGSM.Panel.Server.Application.Core.Accounts;
using BytexDigital.RGSM.Panel.Server.Application.Core.Authentication.ApiKeys;
using BytexDigital.RGSM.Panel.Server.Application.Core.Groups;
using BytexDigital.RGSM.Panel.Server.Application.Core.Settings;
using BytexDigital.RGSM.Panel.Server.Application.Core.Steam;

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
                .AddScoped<SteamLoginService>()
                .AddScoped<ApiKeyService>()
                .AddScoped<NodeService>()
                .AddScoped<GroupService>();

            return services;
        }
    }
}
