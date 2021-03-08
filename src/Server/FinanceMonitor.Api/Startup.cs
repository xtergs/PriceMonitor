using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using Dapper;
using FinanceMonitor.Api.Filters;
using FinanceMonitor.Api.Jobs;
using FinanceMonitor.Api.MessageHandlers;
using FinanceMonitor.Api.Models;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.DAL.Services;
using FinanceMonitor.DAL.Services.Interfaces;
using FinanceMonitor.Messages;
using HealthChecks.UI.Client;
using IdentityServer4.AccessTokenValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Serilog;

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
            // services.AddSingleton<Serilog.Core.Logger>(container => new LoggerConfiguration()
            //     //.ConfigureEnrichers(container)
            //     .ConfigureElk("http://elk:9200", Configuration.GetSection("Logging:Elk"), container)
            //     .ConfigureSelfLog(container)
            //     .CreateLogger());
            
            var rebusConfig = new RebusConfig();
            Configuration.Bind("Rebus", rebusConfig);

            services.Configure<StockOptions>(x =>
            {
                x.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            });

            services.AddScoped<IStockRepository, StockRepository>();
            services.AddScoped<IYahooApiService, YahooApiService>();
            services.AddScoped<IUserStockService, UserStockService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IManagementRepository, ManagementRepository>();

            services.AddMediatR(typeof(Stock).GetTypeInfo().Assembly);

            services.AddControllers()
                .AddJsonOptions(opts => { opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

            ConfigureSwagger(services);

            ConfigureQuartz(services);

            SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);

            services.AddCors();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // base-address of your identityserver
                    options.Authority = Configuration.GetSection("Identity")["Authority"];

                    options.Audience = "api";

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                    };
                    
                    options.RequireHttpsMetadata = false;
                });

            services.AutoRegisterHandlersFromAssemblyOf<UserCreatedHandler>();
            services.AddRebus(configure =>
            {
                configure.Transport(t =>
                    t.UseRabbitMq(rebusConfig.RabbitMQConnection, "api").ClientConnectionName("api"));
                configure.Routing(r => r.TypeBased().Map<SymbolHistoryChanged>("charts"));

                return configure;
            });

            AddCustomHealthCheck(services, Configuration);
        }

        private static void ConfigureQuartz(IServiceCollection services)
        {
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
                        .WithCronSchedule("0/20 * * ? * * *");
                });

                q.ScheduleJob<ProcessDailyDataJob>(trigger =>
                {
                    trigger.WithIdentity("ProcessDailyDataJob")
                        .StartAt(DateTimeOffset.UtcNow.AddSeconds(30))
                        .WithCronSchedule("0 0 * ? * * *");
                });
                
                q.ScheduleJob<CalculateFullHistoryGraphicJob>(trigger =>
                {
                    trigger.WithIdentity("CalculateFullHistoryGraphicJob")
                        .StartAt(DateTimeOffset.UtcNow.AddSeconds(60))
                        .WithCronSchedule("0 0 * ? * * *");
                });
            });

            services.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "FinanceMonitor.Api", Version = "v1"});
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:5002/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:5002/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                {"scope2", "swagger api"}
                            }
                        }
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinanceMonitor.Api v1");
                    c.OAuthClientId("swagger");
                    c.OAuthAppName("Demo API - Swagger");
                    c.OAuthClientSecret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0");
                    c.OAuthUsePkce();
                });
            }
            
            app.UseSerilogRequestLogging(); 

            app.ApplicationServices.UseRebus(async bus => { await bus.Subscribe<UserCreatedHandler>(); });

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            
            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        public static IServiceCollection AddCustomHealthCheck(IServiceCollection services,
            IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder
                .AddSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    name: "ApiDb-check",
                    tags: new string[] { "apidb" })
                .AddRabbitMQ(configuration["Rebus:RabbitMQConnection"],
                    name: "Api-RabbitMQ-check",
                    tags: new []{"rabbitMQ"});

            return services;
        }
    }
}