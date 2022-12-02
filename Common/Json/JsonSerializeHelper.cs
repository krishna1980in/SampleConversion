using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;

namespace Infrastructure.Common.Json
{
    public static class JsonSerializeHelper
    {
        public static string SerializeObject(object value, JsonSerializerOptions jsonSerializerOptions =null)
        {
           
                var options = new JsonSerializerSettings {
                    MaxDepth = 1000,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None,
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,

                };
                return JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented, options);          
        }

        public static T DeSerializeObject<T>(string value, JsonSerializerOptions jsonSerializerOptions = null)
        {
            if (string.IsNullOrEmpty(value)) {
                return default;
            }
            var options = new JsonSerializerSettings {
                MaxDepth = 1000,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None,
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,

            };
            return JsonConvert.DeserializeObject<T>(value, options);

        }
    }
}
