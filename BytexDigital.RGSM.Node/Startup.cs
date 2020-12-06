using System;
using System.Collections.Generic;

using BytexDigital.Common.Errors.AspNetCore.Extensions;
using BytexDigital.RGSM.Node.Persistence;

using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            //services
            //    .AddSingleton<ServerStateService>()
            //    .AddSingleton<FileSystemService>()
            //    .AddScoped<WorkTasksService>()
            //    .AddScoped<NodeSettingsService>()
            //    .AddScoped<NodeService>()

            //    // Arma 3
            //    .AddScoped<Application.Games.Arma3.Services.CreationService>()
            //    .AddScoped<Application.Games.Arma3.Services.StatusService>();

            services.AddUniformCommonErrorResponses();

            // Automapper
            //services.AddAutoMapper(typeof(DefaultProfile).Assembly, typeof(NodeProfile).Assembly);

            // Settings
            //services.Configure<NodeOptions>(Configuration.GetSection("Node"));

            // Mediator
            //services.AddMediatR(typeof(GetDirectoryQuery).Assembly)
            //    .AddScoped(typeof(IPipelineBehavior<,>), typeof(DbTransactionBehavior<,>))
            //    .AddScoped(typeof(IPipelineBehavior<,>), typeof(FluentValidationPipelineBehavior<,>));

            // Connection to local db
            services.AddDbContext<NodeDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")).UseLazyLoadingProxies());

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MetadataAddress = $"{Configuration["Panel:BaseUri"]}/.well-known/openid-configuration";
                    options.Audience = "rgsm";

                    options.TokenValidationParameters.ValidIssuer = Configuration["Panel:BaseUri"];
                    options.TokenValidationParameters.ValidAudience = "rgsm";
                });

            services.AddControllers()
                /*.AddFluentValidation(options => options
                    .RegisterValidatorsFromAssemblyContaining<GetDirectoryQuery.Validator>())*/
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy => policy.WithOrigins(Configuration["Panel:BaseUri"]));
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
                            AuthorizationUrl = new Uri($"{Configuration["Panel:BaseUri"]}/connect/authorize"),
                            TokenUrl = new Uri($"{Configuration["Panel:BaseUri"]}/connect/token"),
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //using (var scope = app.ApplicationServices.GetRequiredService<IServiceProvider>().CreateScope())
            //{
            //    var nodeService = scope.ServiceProvider.GetRequiredService<NodeService>();
            //    var db = scope.ServiceProvider.GetRequiredService<NodeDbContext>();

            //    if (!env.IsDevelopment())
            //    {
            //        db.Database.Migrate();
            //    }

            //    nodeService.EnsureLocalSettingsCreatedAsync().GetAwaiter().GetResult();
            //    nodeService.EnsureNodeRegisteredAsync().GetAwaiter().GetResult();
            //}

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
