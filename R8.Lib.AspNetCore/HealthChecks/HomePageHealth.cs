using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace R8.Lib.AspNetCore.HealthChecks
{
    public class HomePageHealth : IHealthCheck
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomePageHealth(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var myUrl = request.Scheme + "://" + request.Host;

            using var client = new HttpClient();
            var response = await client.GetAsync(myUrl, cancellationToken);
            var pageContents = await response.Content.ReadAsStringAsync();
            return pageContents.Contains(".NET Bot Black Sweatshirt")
                ? HealthCheckResult.Healthy("The check indicates a healthy result.")
                : HealthCheckResult.Unhealthy("The check indicates an unhealthy result.");
        }
    }
}