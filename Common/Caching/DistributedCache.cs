using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Caching
{
    public class DistributedCache : ICache
    {
        #region " Public "

        public DistributedCache(IDistributedCache cache)
        {
            //this.settings = settings;
            _cache = cache;
            Init();
        }

        public async Task SetStringAsync(string key, string value)
        {

            await _cache.SetStringAsync(key, value).ConfigureAwait(false);
        }

        public async Task SetStringAsync(string key, string value, int cacheTime)
        {
            DateTime expireDate;
            if (cacheTime == 99)
                expireDate = DateTime.Now + TimeSpan.FromSeconds(30);
            else
                expireDate = DateTime.Now + TimeSpan.FromMinutes(cacheTime);


            var options = new DistributedCacheEntryOptions()
           .SetSlidingExpiration(TimeSpan.FromSeconds(expireDate.Ticks));


            await _cache.SetStringAsync(key, value, options).ConfigureAwait(false); 
        }

        public async Task SetObjectAsync(string key, object value)
        {
            await _cache.SetStringAsync(key, JsonConvert.SerializeObject(value)).ConfigureAwait(false); 
        }

        public async Task SetObjectAsync(string key, object value, int cacheTime)
        {
            DateTime expireDate;
            if (cacheTime == 99)
                expireDate = DateTime.Now + TimeSpan.FromSeconds(30);
            else
                expireDate = DateTime.Now + TimeSpan.FromMinutes(cacheTime);

            var options = new DistributedCacheEntryOptions()
       .SetSlidingExpiration(TimeSpan.FromSeconds(expireDate.Ticks));

            await _cache.SetStringAsync(key, JsonConvert.SerializeObject(value), options).ConfigureAwait(false);
        }

        public async Task<string> GetStringAsync(string key)
        {
            var value = await _cache.GetStringAsync(key).ConfigureAwait(false); 
            return string.IsNullOrEmpty(value) ? "" : value.ToString();
        }

        public async Task<T> GetObjectAsync<T>(string key)
        {
            var value = await _cache.GetStringAsync(key).ConfigureAwait(false);
            return string.IsNullOrEmpty(value) ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public async Task<bool> ExistAsync(string key)
        {
            var value = await _cache.GetStringAsync(key).ConfigureAwait(false);
            return string.IsNullOrEmpty(value) ? false : true;
        }

        public async Task DeleteAsync(string key)
        {
            await _cache.RefreshAsync(key).ConfigureAwait(false);
        }

        public List<string> ListKeys()
        {

            return null;
        }

        public Dictionary<string, string> ListKeyValues()
        {

            return null;
        }

        #endregion



        private readonly IDistributedCache _cache;
        //private AzureCacheSettings settings;

        private void Init()
        {
            //connection = ConnectionMultiplexer.Connect(settings.ConnectionString);
            //database = connection.GetDatabase();
            //server = connection.GetServer(connection.GetEndPoints().First());
        }
    }
}
