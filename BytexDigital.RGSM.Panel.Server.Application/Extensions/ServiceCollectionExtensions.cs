﻿using BytexDigital.RGSM.Panel.Server.Application.Core;

using Microsoft.Extensions.DependencyInjection;

namespace BytexDigital.RGSM.Panel.Server.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services
                .AddScoped<DatabaseDefaultsService>()
                .AddScoped<AccountsService>()
                .AddScoped<SettingsService>()
                .AddScoped<SteamCredentialsService>()
                .AddScoped<AuthenticationService>()
                .AddScoped<NodesService>();

            return services;
        }
    }
}
