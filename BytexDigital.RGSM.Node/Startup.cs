using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Autofac;

using AutoMapper;

using BytexDigital.Common.Errors.AspNetCore.Extensions;
using BytexDigital.Common.Errors.MediatR;
using BytexDigital.RGSM.Node.Application.Core;
using BytexDigital.RGSM.Node.Application.Core.Arma3;
using BytexDigital.RGSM.Node.Application.Core.Authorization.Requirements;
using BytexDigital.RGSM.Node.Application.Core.Commands;
using BytexDigital.RGSM.Node.Application.Core.Scheduling;
using BytexDigital.RGSM.Node.Application.Core.SteamCmd;
using BytexDigital.RGSM.Node.Application.Core.SteamCmd.Commands;
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
                .AddScoped<ArmaServerService>()
                .AddScoped<PermissionsService>()
                .AddScoped<ServerSetupService>()
                .AddScoped<SchedulersService>()
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
            services.AddSingleton<IAuthorizationHandler, PermissionRequirement.Handler>();

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
                options.DefaultRequestHeaders.TryAddWithoutValidation("Node-Api-Key", Configuration["NodeSettings:Master:ApiKey"]);
                options.BaseAddress = new Uri(Configuration["NodeSettings:Master:BaseUri"]);
            });

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MasterApi"));

            // Connection to local db
            services.AddDbContext<NodeDbContext>(options =>
                options
                    .UseSqlite(Configuration.GetConnectionString("DefaultConnection"), o => o.MigrationsAssembly(typeof(NodeDbContext).Assembly.GetName().Name))
                    .UseLazyLoadingProxies());

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MetadataAddress = $"{Configuration["NodeSettings:Master:BaseUri"]}/.well-known/openid-configuration";
                    options.Audience = "rgsm";

                    options.TokenValidationParameters.ValidIssuer = Configuration["NodeSettings:Master:BaseUri"];
                    options.TokenValidationParameters.ValidAudience = "rgsm";
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
                options.AddDefaultPolicy(policy => policy.WithOrigins(Configuration["NodeSettings:Master:BaseUri"]));
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
                            AuthorizationUrl = new Uri($"{Configuration["NodeSettings:Master:BaseUri"]}/connect/authorize"),
                            TokenUrl = new Uri($"{Configuration["NodeSettings:Master:BaseUri"]}/connect/token"),
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
                    scope.ServiceProvider.GetRequiredService<NodeDbContext>().Database.Migrate();

                    await scope.ServiceProvider.GetRequiredService<ServerSetupService>().EnsureCorrectSetupAllAsync();
                    await scope.ServiceProvider.GetRequiredService<ServerStateRegister>().InitializeAsync();
                    await scope.ServiceProvider.GetRequiredService<SteamDownloadService>().InitializeAsync();
                    await scope.ServiceProvider.GetRequiredService<SchedulerHandler>().InitializeAsync();
                }).GetAwaiter().GetResult();
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
