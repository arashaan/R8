using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace R8.Lib.AspNetCore.WebApi
{
    public static class BuilderExtension
    {
        public static IApplicationBuilder UseWebApiAuthentication(this IApplicationBuilder applicationBuilder, string token = "token", string message = "message", string status = "status")
        {
            return applicationBuilder.UseMiddleware<WebApiAuthenticationMiddleware>();
        }
    }

    public class WebApiAuthenticationMiddleware : IMiddleware
    {
        private readonly RequestDelegate _next;
        public string Token { get; set; } = "token";
        public string Message { get; set; } = "message";
        public string Status { get; set; } = "status";

        public WebApiAuthenticationMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var bodyStream = context.Response.Body;
            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;
            await _next(context).ConfigureAwait(false);

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(responseBody)) return;

            if (context.Response.ContentType?.Contains("application/json") == true)
            {
                var json = JToken.Parse(responseBody);

                var token = json[Token].ToString();
                var status = int.Parse(json[Status].ToString());
                var message = json[Message].ToString();

                if (!string.IsNullOrEmpty(token))
                {
                    //var name = (await settingService.HomeIdentity().ConfigureAwait(false)).FullName;
                    json[Message] = "check";
                    json[Status] = 1;
                }
                else
                {
                    json[Status] = 0;
                    json[Message] = "Unauthorized access";
                }

                var newBody = JsonConvert.SerializeObject(json);
                var newBodyStream = new MemoryStream(Encoding.UTF8.GetBytes(newBody));
                await newBodyStream.CopyToAsync(bodyStream).ConfigureAwait(false);
            }
            else
            {
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(bodyStream).ConfigureAwait(false);
            }
        }
    }
}