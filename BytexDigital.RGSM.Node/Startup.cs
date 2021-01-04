using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Autofac;

using AutoMapper;

using BytexDigital.ErrorHandling.AspNetCore.Server.Extensions;
using BytexDigital.ErrorHandling.MediatR;
using BytexDigital.RGSM.Node.Application.Authentication;
using BytexDigital.RGSM.Node.Application.Core;
using BytexDigital.RGSM.Node.Application.Core.Arma3;
using BytexDigital.RGSM.Node.Application.Core.Authorization.Requirements;
using BytexDigital.RGSM.Node.Application.Core.Commands;
using BytexDigital.RGSM.Node.Application.Core.Features.FileSystem;
using BytexDigital.RGSM.Node.Application.Core.Features.Scheduling;
using BytexDigital.RGSM.Node.Application.Core.Infrastructure;
using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Core.Servers.Commands;
using BytexDigital.RGSM.Node.Application.Core.Steam;
using BytexDigital.RGSM.Node.Application.Core.Steam.Commands;
using BytexDigital.RGSM.Node.Application.Mappings;
using BytexDigital.RGSM.Node.Application.Mediator;
using BytexDigital.RGSM.Node.Application.Options;
using BytexDigital.RGSM.Node.Persistence;

using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace BytexDigital.RGSM.Node
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddScoped<ServersService>()
                .AddScoped<ConnectivityService>()
                .AddScoped<ArmaServerService>()
                .AddScoped<PermissionsService>()
                .AddScoped<ServerIntegrityService>()
                .AddScoped<SchedulersService>()
                .AddScoped<WorkshopManagerService>()
                .AddSingleton<FileSystemService>()
                .AddSingleton<SteamDownloadService>()
                .AddSingleton<ServerStateRegister>()
                .AddSingleton<ScopeService>()
                .AddSingleton<MasterApiService>();

            services
                .AddSingleton<SchedulerHandler>()
                .AddSingleton<IHostedService>(x => x.GetRequiredService<SchedulerHandler>());

            services.AddUniformCommonErrorResponses();

            // Authorization
            // Add AuthorizationHandlers
            foreach (var handlerType in typeof(PermissionRequirement.Handler).Assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(IAuthorizationHandler))))
            {
                services.AddScoped(typeof(IAuthorizationHandler), handlerType);
            }

            // Automapper
            services.AddAutoMapper(typeof(NodeProfile).Assembly);

            // Settings
            services.Configure<NodeOptions>(Configuration.GetSection("NodeSettings"));

            // Mediator
            services.AddMediatR(typeof(UpdateAppCmd).Assembly)
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(ScopeBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(FluentValidationPipelineBehavior<,>));

            services.AddHttpClient("MasterApi", options =>
            {
                options.DefaultRequestHeaders.TryAddWithoutValidation(ApiKeyAuthenticationHandler.HEADER_NAME, Configuration["NodeSettings:MasterOptions:ApiKey"]);
                options.BaseAddress = new Uri(Configuration["NodeSettings:MasterOptions:BaseUri"]);
            });

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MasterApi"));

            // Connection to local db
            services.AddDbContext<NodeDbContext>(options =>
                options
                    .UseSqlite(Configuration.GetConnectionString("DefaultConnection"), o => o.MigrationsAssembly(typeof(NodeDbContext).Assembly.GetName().Name))
                    .UseLazyLoadingProxies());

            services.AddAuthentication("API_KEY_OR_JWT")
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.NODE_AUTHENTICATION_SCHEME, null)
                .AddJwtBearer(options =>
                {
                    options.MetadataAddress = $"{Configuration["NodeSettings:MasterOptions:BaseUri"]}/.well-known/openid-configuration";
                    options.Audience = "rgsm";

                    options.TokenValidationParameters.ValidIssuer = Configuration["NodeSettings:MasterOptions:BaseUri"];
                    options.TokenValidationParameters.ValidAudience = "rgsm";
                })
                .AddPolicyScheme("API_KEY_OR_JWT", "API_KEY_OR_JWT", options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        if (context.Request.Headers.ContainsKey(ApiKeyAuthenticationHandler.HEADER_NAME))
                        {
                            return ApiKeyAuthenticationOptions.NODE_AUTHENTICATION_SCHEME;
                        }
                        else
                        {
                            return JwtBearerDefaults.AuthenticationScheme;
                        }
                    };
                });

            services.AddControllers()
                .AddFluentValidation(options => options
                    .RegisterValidatorsFromAssemblyContaining<GetServersQuery>())
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy => policy.RequireClaim("scope", "rgsm.user"));
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy => policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("rgsm-node", new OpenApiInfo { Title = "RGSM Node API", Version = "v1" });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{Configuration["NodeSettings:MasterOptions:BaseUri"]}/connect/authorize"),
                            TokenUrl = new Uri($"{Configuration["NodeSettings:MasterOptions:BaseUri"]}/connect/token"),
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
                    ] = new[] { "rgsm-node" }
                });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceProvider>().CreateScope())
            {
                Task.Run(async () =>
                {
                    await scope.ServiceProvider.GetRequiredService<NodeDbContext>().Database.MigrateAsync();
                    await scope.ServiceProvider.GetRequiredService<IMediator>().Send(new PerformStartupCmd());
                }).GetAwaiter().GetResult();

                //Task.Run(async () =>
                //{
                //    // Migrate the database if necessary
                //    scope.ServiceProvider.GetRequiredService<NodeDbContext>().Database.Migrate();

                //    // Make sure we are connected to the masterserver with a valid api key
                //    if (!await scope.ServiceProvider.GetRequiredService<ConnectivityService>().IsConnectedToMasterAsync())
                //    {
                //        throw new NoMasterConnectionException();
                //    }

                //    // Initialize services and other startup related jobs
                //    await scope.ServiceProvider.GetRequiredService<ServerIntegrityService>().EnsureCorrectSetupAllAsync();
                //    await scope.ServiceProvider.GetRequiredService<ServerStateRegister>().InitializeAsync();
                //    await scope.ServiceProvider.GetRequiredService<SteamDownloadService>().InitializeAsync();
                //    await scope.ServiceProvider.GetRequiredService<SchedulerHandler>().InitializeAsync();
                //}).GetAwaiter().GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/rgsm-node/swagger.json", "RGSM Node API"); ;
                    options.OAuthClientId("rgsm-panel");
                    options.OAuthClientSecret("");
                    options.OAuthAppName("RGSM Node - Swagger");
                    options.OAuthUsePkce();
                });
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
