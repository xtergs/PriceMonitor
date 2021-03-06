﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Diagnostics;
using FinanceMonitor.Identity.Data;
using FinanceMonitor.Identity.Models;
using FinanceMonitor.Messages;
using HealthChecks.UI.Client;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Serilog;

namespace FinanceMonitor.Identity
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var rebusConfig = new RebusConfig();
            Configuration.Bind("Rebus", rebusConfig);
            
             Debug.WriteLine(rebusConfig.RabbitMQConnection);

            services.AddControllersWithViews();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "FinanceMonitor.Identity", Version = "v1"});
            });


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(x =>
                {
                    x.Password.RequireDigit = false;
                    x.Password.RequiredLength = 0;
                    x.Password.RequireLowercase = false;
                    x.Password.RequireUppercase = false;
                    x.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddAspNetIdentity<ApplicationUser>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });

            services.AddRebus(configure =>
            {
                configure.Transport(t => t.UseRabbitMq(rebusConfig.RabbitMQConnection, "identity")
                    .ClientConnectionName("identity"));
                configure.Routing(r => r.TypeBased().MapAssemblyOf<Message>("api"));
                return configure;
            });

            services.AddCors();
            
            AddCustomHealthCheck(services, Configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinanceMonitor.Identity v1")
                );
            }
            
            app.UseSerilogRequestLogging();

            app.ApplicationServices.UseRebus();

            app.UseStaticFiles();

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            
            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            
            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
        }
        
        public static IServiceCollection AddCustomHealthCheck(IServiceCollection services,
            IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder
                .AddSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    name: "IdentityDb-check",
                    tags: new string[] { "identitydb" })
                .AddRabbitMQ(configuration["Rebus:RabbitMQConnection"],
                    name: "Identity-RabbitMQ-check",
                    tags: new []{"rabbitMQ"});

            return services;
        }
    }
}