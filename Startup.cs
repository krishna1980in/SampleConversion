using Google.Cloud.PubSub.V1;
using Infrastructure.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SampleConversion.Helper;

namespace SampleConversion
{
     public class Startup
    {
        public Startup(IConfiguration configuration)
        {
           _appConfiguration = AppConfigurations.Get();
        }

        public IConfiguration Configuration { get; }
  
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;
      
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomAPIController();
            services.AddIoC();
            services.AddCustomHttpCleint();
            services.AddCustomInMemory();
            services.AddCustomSwagger();
            services.AddCustomHealthCheck();
            services.AddCustomCors();
            services.AddCustomCompression();
            services.AddCustomHsts();
            services.AddCustomIpRateLimit();
            services.AddIntegrationServices(Configuration);
            services.AddEventBus(Configuration);
             services.AddSchedular(Configuration);

            // Google pub sub start
            // Add framework services.
            services.AddOptions();
            services.Configure<PubsubOptions>(
                Configuration.GetSection("Pubsub"));
            services.AddMvc();
            // Add Pubsub publisher.
            services.AddSingleton((provider) =>
            {
                var options = provider.GetService<IOptions<PubsubOptions>>()
                    .Value;
                
                return PublisherClient.CreateAsync(
                    new TopicName(options.ProjectId, options.TopicId)).Result;
            });

            services.AddSingleton((provider) =>
            {
                var options = provider.GetService<IOptions<PubsubOptions>>()
                    .Value;
                
                return SubscriberClient.CreateAsync(
                    new SubscriptionName(options.ProjectId, options.SubscriptionId)).Result;
            });
            // Google pub sub End
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
             loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'");
            
            var App = new CustomExtensionStartup();
            
            App.ConfigureDefualt(app);
        }

    }
}
