using BytexDigital.RGSM.Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace BytexDigital.RGSM.Panel.Server.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services
                .AddScoped<DatabaseDefaultsService>()
                .AddScoped<AccountsService>();

            return services;
        }
    }
}
