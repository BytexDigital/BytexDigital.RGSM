using System;
using System.Collections.Generic;

using AutoMapper;

using BytexDigital.RGSM.Application.Behaviors;
using BytexDigital.RGSM.Application.Mapping;
using BytexDigital.RGSM.Application.Services;
using BytexDigital.RGSM.Domain.Entities;
using BytexDigital.RGSM.Panel.Server.Application.Commands.Authentication;
using BytexDigital.RGSM.Panel.Server.Application.Extensions;
using BytexDigital.RGSM.Panel.Server.Common.Filters;
using BytexDigital.RGSM.Panel.Server.Common.IdentityServer;
using BytexDigital.RGSM.Persistence;

using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Validation;

using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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

            services.AddMediatR(typeof(LoginCmd).Assembly);

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DbTransactionBehavior<,>));

            services.AddAutoMapper(typeof(DefaultProfile).Assembly);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")).UseLazyLoadingProxies());

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("rgsm-panel-api", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "RGSM Panel API", Version = "v1" });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{Configuration["BaseUri"]}/connect/authorize"),
                            TokenUrl = new Uri($"{Configuration["BaseUri"]}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "ID" },
                                { "profile", "User profile data" },
                                { "rgsm", "Audience scope" },
                                { "rgsm.user", "Access to the RGSM panel as a user" }
                            }
                        }
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        }
                    ] = new[] { "rgsm-panel" }
                });
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
            })
                .AddSignInManager()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/_/login";
                options.LogoutPath = "/_/logout";
            });

            services.AddHttpContextAccessor();

            services
                .AddIdentityServer(options =>
                {
                    options.UserInteraction.LoginUrl = "/_/login";
                    options.UserInteraction.LogoutUrl = "/_/logout";
                })
                .AddRedirectUriValidator<RedirectUriValidator>()
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
                            .WithRedirectUri($"{Configuration["BaseUri"]}/swagger/oauth2-redirect.html")
                            .WithLogoutRedirectUri("/authentication/logout-callback")
                            .WithScopes("rgsm", "rgsm.user", "rgsm.app");
                    });

                    options.ApiScopes.Add(new ApiScope(IdentityServerConstants.StandardScopes.OfflineAccess));
                    options.ApiScopes.Add(new ApiScope("rgsm.user", "Identifies the token holder as a user."));
                    options.ApiScopes.Add(new ApiScope("rgsm.app", "Identifies the token holder as an application."));
                });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityServerJwt()
                .AddIdentityCookies();

            services.AddTransient<IRedirectUriValidator, RedirectUriValidator>();

            // We need to post configure the JWT options because "AddIdentityServerJwt" adds an Audience value, which we do not want to use.
            services.PostConfigureAll<JwtBearerOptions>(options =>
            {
                options.Audience = "rgsm";
                options.TokenValidationParameters.ValidAudience = "rgsm";
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

                dbDefaultsService.EnsureSystemAdministratorGroupExistsAsync().GetAwaiter().GetResult();
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

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/rgsm-panel-api/swagger.json", "RGSM Panel API");
                options.OAuthClientId("rgsm-panel");
                options.OAuthClientSecret("");
                options.OAuthAppName("RGSM Panel - Swagger");
                options.OAuthUsePkce();
            });

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
