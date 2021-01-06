using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace R8.AspNetCore
{
    public class AdminIpWhileListMiddleware : IMiddleware
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

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Method != HttpMethod.Get.Method)
            {
                var remoteIp = context.Connection.RemoteIpAddress;
                _logger.LogDebug("Request from Remote IP address: {remoteIp}", remoteIp);

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
                        "Forbidden Request from Remote IP address: {remoteIp}", remoteIp);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            await _next(context);
        }
    }
}