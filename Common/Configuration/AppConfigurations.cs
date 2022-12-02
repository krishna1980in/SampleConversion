using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Azure.Identity;
using Azure.Core;

namespace Infrastructure.Common
{
    public static class AppConfigurations
    {
        private static readonly ConcurrentDictionary<string, IConfigurationRoot> _configurationCache;

        static AppConfigurations()
        {
            _configurationCache = new ConcurrentDictionary<string, IConfigurationRoot>();
        }

        public static IConfigurationRoot Get(string path = null, string environmentName = null, bool addUserSecrets = false)
        {
            if (string.IsNullOrEmpty(path)) {
                path = AppContext.BaseDirectory;
            }
            if (!string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("Isfunction"))) {
                path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..");
            }
            var cacheKey = path + "#" + environmentName + "#" + addUserSecrets;
            return _configurationCache.GetOrAdd(
                cacheKey,
                _ => BuildConfiguration(path, environmentName, addUserSecrets)
            );
        }

        private static IConfigurationRoot BuildConfiguration(string path, string environmentName = null, bool addUserSecrets = false)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            if (!string.IsNullOrWhiteSpace(environmentName)) {
                builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            }

            builder = builder.AddEnvironmentVariables();

            if (addUserSecrets) {
                builder.AddUserSecrets(typeof(AppConfigurations).Assembly, optional: true);
            }
            
            var config = builder.Build();


            if (config.GetValue<bool>("UseVault", false)) {
                builder.AddAzureKeyVault(
                    new Uri($"https://{config["KeyVaultName"]}.vault.azure.net/"),
                    new DefaultAzureCredential());
            }
            return builder.Build();
        }
    }
    public class ConfigurationManager
    {

        public AppSettings AppSettings = new AppSettings();
    }
    public class AppSettings
    {

        public string this[string index] {

            get {

                return AppConfigurations.Get()[index];
            }


        }
    }

}
