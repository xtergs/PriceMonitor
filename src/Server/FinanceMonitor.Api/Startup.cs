using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FinanceMonitor.Api.Jobs;
using FinanceMonitor.Api.MessageHandlers;
using FinanceMonitor.Api.Models;
using FinanceMonitor.DAL.Repositories;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.DAL.Services;
using FinanceMonitor.DAL.Services.Interfaces;
using FinanceMonitor.Messages;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Quartz;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Message = Rebus.Messages.Message;

namespace FinanceMonitor.Api
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
            var rebusConfig = new RebusConfig();
            Configuration.Bind("Rebus", rebusConfig);

            services.Configure<StockOptions>(x =>
            {
                x.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            });
            services.Configure<MigrationOptions>(x =>
            {
                x.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
                x.FilePath = Path.Combine(System.IO.Path.GetDirectoryName(
                        System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "migration.sql")
                    .Substring(6);
            });

            services.AddScoped<IStockRepository, StockRepository>();
            services.AddScoped<IYahooApiService, YahooApiService>();
            services.AddScoped<IUserStockService, UserStockService>();
            services.AddSingleton<MigrationService>();


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "FinanceMonitor.Api", Version = "v1"});
            });

            services.AddQuartz(x =>
            {
                x.UseMicrosoftDependencyInjectionScopedJobFactory(c => { c.AllowDefaultConstructor = true; });
            });

            services.AddQuartz(q =>
            {
                q.ScheduleJob<PullHistoryJob>(trigger =>
                {
                    trigger.WithIdentity("PullHistoryJob")
                        .StartAt(DateTimeOffset.UtcNow.AddSeconds(7))
                        .WithCronSchedule("0 * * ? * * *");
                });

                q.ScheduleJob<PullDailyInfoJob>(trigger =>
                {
                    trigger.WithIdentity("PullDailyInfoJob")
                        .StartAt(DateTimeOffset.UtcNow.AddSeconds(5))
                        .WithCronSchedule("0 * * ? * * *");
                });
            });

            services.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });

            SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);

            services.AddCors();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    // base-address of your identityserver
                    options.Authority = "https://localhost:5002";

                    options.ApiName = "api";
                });

            services.AutoRegisterHandlersFromAssemblyOf<UserCreatedHandler>();
            services.AddRebus(configure =>
            {
                configure.Transport(t => t.UseRabbitMq(rebusConfig.RabbitMQConnection, "api").ClientConnectionName("api"));
                return configure;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinanceMonitor.Api v1"));
            }
            
            app.ApplicationServices.UseRebus(async bus =>
            {
                await bus.Subscribe<UserCreatedHandler>();
            });

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}