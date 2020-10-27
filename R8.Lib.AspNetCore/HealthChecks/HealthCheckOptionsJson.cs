using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Newtonsoft.Json;

using System;
using System.Linq;
using System.Net.Mime;

namespace R8.Lib.AspNetCore.HealthChecks
{
    public class HealthCheckOptionsJson : HealthCheckOptions
    {
        public HealthCheckOptionsJson()
        {
            ResponseWriter = async (context, report) =>
            {
                var jsonObj = new
                {
                    status = report.Status.ToString(),
                    errors = report.Entries.Select(e =>
                        new
                        {
                            key = e.Key,
                            value = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                        })
                };
                var json = JsonConvert.SerializeObject(jsonObj);
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(json);
            };
        }
    }
}