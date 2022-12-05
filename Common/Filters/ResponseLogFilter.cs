namespace Infrastructure.Common
{
   
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Microsoft.AspNetCore.Mvc;

    public class ResponseLogFilter : IAsyncResultFilter
    {
        private readonly ILogger<ResponseLogFilter> logger;

        public ResponseLogFilter(ILogger<ResponseLogFilter> logger)
        {
            this.logger = logger;
        }
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentException("Filter Context is null");
            }

            if (context.Result is ObjectResult result)
            {
                var responseJosn = JsonSerializer.Serialize(result.Value);
                logger.LogInformation($" Response - { responseJosn}");
            }
            if (next != null)
            {
                await next().ConfigureAwait(false);
            }
        }

       
    }
}
