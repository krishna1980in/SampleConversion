﻿using Autofac.Extensions.DependencyInjection;
using Autofac;
using AspNetCoreRateLimit;
using Infrastructure.Common.Filters;
using Infrastructure.Common.StartUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Common.GCP.Scheduler;

namespace Infrastructure.Common
{
    public static class StartUpExtensionMethods
    {

        public static IServiceCollection AddCustomHsts(this IServiceCollection services)
        { 
            services.AddHsts(options => {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
        

            return services;
        }
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services)
        {
            var hcBuilder = services.AddHealthChecks();
            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            return services;
        }
        public static IServiceCollection AddCustomHttpCleint(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            return services;
        }
        public static IServiceCollection AddCustomInMemory(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();

            return services;
        }
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            var _appConfiguration = AppConfigurations.Get();
            if (Convert.ToBoolean(_appConfiguration["EnableSwagger"])) {
                var pathBase = AppConfigurations.Get()["PATH_BASE"];
                services.AddSwaggerGen(c => {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = pathBase, Version = "v1" });
                });
                
            }
            return services;
        }

        public static IServiceCollection AddCustomAPIController(this IServiceCollection services)
        {
            var _appConfiguration = AppConfigurations.Get();
            services.AddControllers(config => {
                if (Convert.ToBoolean(_appConfiguration["EnableRequestLogFilter"])) {
                    config.Filters.Add(typeof(RequestLogFilter));
                }
                if (Convert.ToBoolean(_appConfiguration["EnableResponseLogFilter"])) {
                    config.Filters.Add(typeof(ResponseLogFilter));
                }
                config.Filters.Add(typeof(HttpGlobalExceptionFilter));
                config.EnableEndpointRouting = false;
                config.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.HttpNoContentOutputFormatter>();
                AutherizationFilterExtention.AddCustomAutherization(config);
            }).AddNewtonsoftJson(
            options => {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            }
        ).ConfigureApiBehaviorOptions(o => { o.SuppressModelStateInvalidFilter = true; }); ;


            //     .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true)
            //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


           

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            var _defaultCorsPolicyName = AppConfigurations.Get()["CorsPolicyName"];
            var _appConfiguration = AppConfigurations.Get();
            if (_appConfiguration["AllowedHosts"] == "*") {
                services.AddCors(
                 options => options.AddPolicy(
                     _defaultCorsPolicyName,
                     builder => builder
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .SetIsOriginAllowedToAllowWildcardSubdomains()
                         .AllowCredentials()
                 )
             );
            }
            else {
                services.AddCors(
                   options => options.AddPolicy(
                       _defaultCorsPolicyName,
                       builder => builder
                           .WithOrigins(
                               // AllowedHosts :CorsOrigins in appsettings.json can contain more than one address separated by comma.
                               _appConfiguration["AllowedHosts"]
                                   .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                   .ToArray()
                           )
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .SetIsOriginAllowedToAllowWildcardSubdomains()
                           .AllowCredentials()
                   )
               );
            }


            return services;
        }
        public static IServiceCollection AddCustomCompression(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });


            return services;
        }

        public static IServiceCollection AddCustomIpRateLimit(this IServiceCollection services)
        {
            var _appConfiguration = AppConfigurations.Get();
            services.AddMemoryCache();
           services.Configure<IpRateLimitOptions>(_appConfiguration.GetSection("IpRateLimiting"));
           services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
           services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
           services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
           services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
           services.AddInMemoryRateLimiting();

            return services;
        }
    }


    public class CustomExtensionStartup
    {
        public virtual void ConfigureBasePath(IApplicationBuilder app)
        {
            var pathBase = AppConfigurations.Get()["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase)) {
                app.UsePathBase($"/{pathBase}");

                //app.Use((context, next) =>
                //{
                //    context.Request.PathBase = $"/{pathBase}";
                //    return next();
                //});
            }
           

        }
        public virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
        public virtual void ConfigureRoute(IApplicationBuilder app)
        {
            app.UseRouting();
        }
        public virtual void ConfigureCors(IApplicationBuilder app)
        {
            var _defaultCorsPolicyName = AppConfigurations.Get()["CorsPolicyName"];
            app.UseCors(_defaultCorsPolicyName);
        }
        public virtual void ConfigureHttps(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }
        public virtual void ConfigureAddHealthCheck(IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions {
                Predicate = _ => true
            })
              .UseHealthChecks("/", new HealthCheckOptions {
                  Predicate = _ => true,
                  //  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
              });
        }
        public virtual void ConfigureSwagger(IApplicationBuilder app)
        {
            var _appConfiguration = AppConfigurations.Get();
            if (Convert.ToBoolean(_appConfiguration["EnableSwagger"])) {
                var pathBase = AppConfigurations.Get()["PATH_BASE"];
                app.UseSwagger();
                app.UseSwaggerUI(options => {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{pathBase} v1");
                    options.DisplayRequestDuration();
                   // options.RoutePrefix = pathBase;
                });
            }
        }
        public virtual void ConfigureEndPoint(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }

        public virtual void ConfigureIpRateLimiting(IApplicationBuilder app)
        {
            app.UseIpRateLimiting();
        }
        public virtual void ConfigureCompression(IApplicationBuilder app)
        {
            app.UseResponseCompression();
        }
        public virtual void ConfigureSecurityHeaders(IApplicationBuilder app)
        {
                 app.UseSecurityHeadersMiddleware(new SecurityHeadersBuilder()
                .AddDefaultSecurePolicy()
                .AddCustomHeader("X-My-Custom-Header", "So amaze")
              );

        }
        public virtual void ConfigureDefualt(IApplicationBuilder app)
        {
            ConfigureBasePath(app);          
            ConfigureSwagger(app);
            ConfigureCors(app);
            ConfigureAddHealthCheck(app);
            ConfigureCompression(app);
            ConfigureSecurityHeaders(app);
            ConfigureHttps(app);
            ConfigureIpRateLimiting(app);
            ConfigureRoute(app);
            ConfigureAuth(app);
            ConfigureEndPoint(app);
            ServiceHelper.Services = app.ApplicationServices;
        }
          
          
          public static IServiceCollection AddSchedular(this IServiceCollection services, IConfiguration configuration)
           {
           services.AddSingleton<IJobSchedular>(sp =>
                {
                    
                    string Project = "";
                    if (!string.IsNullOrEmpty(configuration.GetSection("GCPJobSchedular")["Project"]))
                    {
                        Project = configuration.GetSection("GCPJobSchedular")["Project"];
                    }

                    string Location = "";
                    if (!string.IsNullOrEmpty(configuration.GetSection("GCPJobSchedular")["Location"]))
                    {
                         Location = configuration.GetSection("GCPJobSchedular")["Location"];
                    }

                    return new JobSchedularGCP(Project, Location);
                });
          return services;
         }
         

                         
     }



}
