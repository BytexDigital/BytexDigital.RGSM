using System;

using AutoMapper;

using BytexDigital.RGSM.Application.Mapping;
using BytexDigital.RGSM.Node.Application.Mapping;
using BytexDigital.RGSM.Node.Application.Shared.Options;
using BytexDigital.RGSM.Node.Application.Shared.Services;
using BytexDigital.RGSM.Node.Persistence;
using BytexDigital.RGSM.Persistence;

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
            services
                .AddSingleton<LocalInstanceCreationService>()
                .AddSingleton<NodeFileSystemService>()
                .AddScoped<ServerService>()
                .AddScoped<NodeSettingsService>()
                .AddScoped<NodeService>();

            // Automapper
            services.AddAutoMapper(typeof(DefaultProfile).Assembly, typeof(NodeProfile).Assembly);

            // Settings
            services.Configure<NodeOptions>(Configuration.GetSection("Node"));

            // Connection to global db
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("PanelConnection")).UseLazyLoadingProxies());

            // Connection to local db
            services.AddDbContext<NodeDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("NodeConnection")).UseLazyLoadingProxies());

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MetadataAddress = $"{Configuration["Panel:BaseUri"]}/.well-known/openid-configuration";
                    options.Audience = "rgsm";

                    options.TokenValidationParameters.ValidIssuer = Configuration["Panel:BaseUri"];
                    options.TokenValidationParameters.ValidAudience = "rgsm";
                });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("rgsm-node", new OpenApiInfo { Title = "RGSM Node API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceProvider>().CreateScope())
            {
                var nodeService = scope.ServiceProvider.GetRequiredService<NodeService>();
                var db = scope.ServiceProvider.GetRequiredService<NodeDbContext>();

                if (!env.IsDevelopment())
                {
                    db.Database.Migrate();
                }

                nodeService.EnsureLocalSettingsCreatedAsync().GetAwaiter().GetResult();
                nodeService.EnsureNodeRegisteredAsync().GetAwaiter().GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/rgsm-node/swagger.json", "RGSM Node API"));
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
