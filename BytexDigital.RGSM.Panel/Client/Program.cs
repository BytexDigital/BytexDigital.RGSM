using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Blazored.Modal;
using Blazored.Toast;

using BytexDigital.RGSM.Panel.Client.Common.Authorization;
using BytexDigital.RGSM.Panel.Client.Common.Core;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BytexDigital.RGSM.Panel.Client
{
    public class Program
    {
#pragma warning disable IDE1006 // Benennungsstile
        public static async Task Main(string[] args)
#pragma warning restore IDE1006 // Benennungsstile
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("BytexDigital.RGSM.Panel.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))

                // We need to provide the jwt to any call since we cannot know what possible endpoints there are for different nodes
                // Maybe add some other kind of validation here so we don't have to attach it to EVERY request?
                //.AddHttpMessageHandler<AnyAddressAuthorizationMessageHandler>();
                .AddHttpMessageHandler<AnyAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped<AnyAddressAuthorizationMessageHandler>();

            // MediatR support
            builder.Services.AddMediatR(typeof(AccountService).Assembly);

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BytexDigital.RGSM.Panel.ServerAPI"));

            // Add custom services
            builder.Services
                .AddScoped<AccountService>()
                .AddScoped<PermissionService>()
                .AddScoped<NodeRegisterService>()
                .AddScoped<NodeService>()
                .AddScoped<GroupService>()
                .AddScoped<ServerService>()
                .AddScoped<ToastService>();

            // Enable options pattern
            builder.Services.AddOptions();

            // Blazored libraries
            builder.Services
                .AddBlazoredModal()
                .AddBlazoredToast();

            // Authorization with our API
            builder.Services.AddApiAuthorization(options =>
            {
                // We need to set the configuration endpoint manually so that we can use a custom client_id instead of the inferred one.
                string clientId = "rgsm-panel";
                options.ProviderOptions.ConfigurationEndpoint = $"_configuration/{clientId}";
            });

            // Add authorization services
            builder.Services.AddAuthorizationCore();

            // Add AuthorizationHandlers
            foreach (var handlerType in typeof(UserGroupRequirement.Handler).Assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(IAuthorizationHandler))))
            {
                builder.Services.AddScoped(typeof(IAuthorizationHandler), handlerType);
            }

            await builder.Build().RunAsync();
        }
    }
}
