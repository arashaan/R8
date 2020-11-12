using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;

namespace R8.AspNetCore.Routing
{
    public static class PageModelExtensions
    {
        public static IAuthenticatedUser GetAuthenticatedUser<T>(this T page) where T : RazorPage
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            var connectedUser = page.User.GetAuthenticatedUser();
            return connectedUser;
        }

        /// <summary>
        /// Gets the current API resource name from HTTP context
        /// </summary>
        /// <param name="httpContext">The HTTP context</param>
        /// <returns>The current resource name if available, otherwise an empty string</returns>
        public static string GetCurrentEndpointUrl(this HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var endpoint = httpContext.GetEndpoint();
            return endpoint?.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName;
        }
    }
}