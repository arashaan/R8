using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

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
            };
        }
    }
}
