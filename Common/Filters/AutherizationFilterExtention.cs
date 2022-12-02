using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Filters
{
   public static class AutherizationFilterExtention
    {
        public static Task AddCustomAutherization(this MvcOptions options)
        {
            var _appConfiguration = AppConfigurations.Get();
            if (Convert.ToBoolean(_appConfiguration["EnableAutherization"])) {
                options.Filters.Add(new AuthorizeFilter());
            }
            return Task.CompletedTask;
        }
    }
}
