using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace R8.AspNetCore
{
    public class AdminIpWhileListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminIpWhileListMiddleware> _logger;
        private readonly string _safelist;

        public AdminIpWhileListMiddleware(
            RequestDelegate next,
            ILogger<AdminIpWhileListMiddleware> logger,
            string safelist)
        {
            _safelist = safelist;
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method != HttpMethod.Get.Method)
            {
                var remoteIp = context.Connection.RemoteIpAddress;
                _logger.LogDebug("Request from Remote IP address: {RemoteIp}", remoteIp);

                var ip = _safelist.Split(';');

                var bytes = remoteIp.GetAddressBytes();
                var badIp = ip
                    .Select(IPAddress.Parse)
                    .All(testIp => !testIp
                        .GetAddressBytes()
                        .SequenceEqual(bytes));

                if (badIp)
                {
                    _logger.LogWarning(
                        "Forbidden Request from Remote IP address: {RemoteIp}", remoteIp);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}