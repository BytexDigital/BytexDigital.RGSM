using System;

using BytexDigital.RGSM.Panel.Server.Application.Extensions;
using BytexDigital.RGSM.Panel.Server.Application.Services;
using BytexDigital.RGSM.Panel.Server.Common.Filters;
using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Panel.Server.Persistence;

using IdentityServer4;
using IdentityServer4.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BytexDigital.RGSM.Panel.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services
                .AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
                {
                    options.ApiResources.Clear(); // Remove default resources ("BytexDigital.RGSM.Panel.ServerAPI")

                    options.ApiResources.Add(new ApiResource("rgsm")
                    {
                        Scopes = new[] { "rgsm" }
                    });

                    options.Clients.AddIdentityServerSPA("rgsm-panel", options =>
                    {
                        options
                            .WithRedirectUri("/authentication/login-callback")
                            .WithLogoutRedirectUri("/authentication/logout-callback")
                            .WithScopes(
                            "rgsm",
                                "rgsm.user",
                                "rgsm.app");
                    });

                    options.ApiScopes.Add(new ApiScope(IdentityServerConstants.StandardScopes.OfflineAccess));
                    options.ApiScopes.Add(new ApiScope("rgsm.user", "Identifies the token holder as a user."));
                    options.ApiScopes.Add(new ApiScope("rgsm.app", "Identifies the token holder as an application."));

                    //var userClient = new IdentityServer4.Models.Client
                    //{
                    //    ClientId = "BytexDigital.RGSM.Panel.Client",
                    //    RedirectUris = new[] { "/authentication/login-callback" },
                    //    PostLogoutRedirectUris = new[] { "/authentication/logout-callback" },
                    //    RequirePkce = true,
                    //    AllowAccessTokensViaBrowser = true,
                    //    RequireConsent = false,
                    //    AllowedGrantTypes = GrantTypes.Code,
                    //    RequireClientSecret = false,
                    //    AllowPlainTextPkce = false,
                    //    AllowedCorsOrigins = Array.Empty<string>(),
                    //    ClientSecrets = new List<Secret>(),
                    //    AllowedScopes = new List<string> {
                    //        IdentityServerConstants.StandardScopes.OpenId,
                    //        IdentityServerConstants.StandardScopes.Profile,
                    //        //"BytexDigital.RGSM.Panel.ServerAPI"
                    //        "rgsm"
                    //        //"rgsm.user"
                    //    }
                    //};

                    // Add this so "DefaultClientRequestParametersProvider" can resolve our client to a response type
                    // (Usually this would be done automatically through configuring the client via the appsettings.json or a 
                    // AddIdentityServerSPA, but since we're manually adding a client, this step needs to be done manually.
                    //userClient.Properties.Add(ApplicationProfilesPropertyNames.Profile, "IdentityServerSPA");
                    //userClient.Properties.Add(ApplicationProfilesPropertyNames.Source, ApplicationProfilesPropertyValues.Configuration);

                    //options.Clients.Add(userClient);
                });
            services
                .AddAuthentication()
                .AddIdentityServerJwt();

            // We need to post configure the JWT options because "AddIdentityServerJwt" adds an Audience value, which we do not want to use.
            services.PostConfigureAll<JwtBearerOptions>(options =>
            {
                options.Audience = "rgsm";
            });

            services.AddControllers(options =>
            {
                options.Filters.Add<ServiceExceptionFilter>();
            });

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceProvider>().CreateScope())
            {
                var dbDefaultsService = scope.ServiceProvider.GetRequiredService<DatabaseDefaultsService>();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (!env.IsDevelopment())
                {
                    db.Database.Migrate();
                }

                dbDefaultsService.EnsureRootAccountExistsAsync().GetAwaiter().GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
