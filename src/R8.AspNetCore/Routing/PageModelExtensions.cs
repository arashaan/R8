using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using R8.AspNetCore.HttpContextExtensions;

namespace R8.AspNetCore.Routing
{
    public static class PageModelExtensions
    {
        /// <summary>
        /// Returns an <see cref="IAuthenticatedUser"/> object according to current authenticated <see cref="HttpContext"/> user.
        /// </summary>
        /// <typeparam name="T">A type of specific <see cref="RazorPage"/>.</typeparam>
        /// <param name="page">A derived class that representing specific razor page model.</param>
        /// <returns>A <see cref="IAuthenticatedUser"/> object.</returns>
        public static IAuthenticatedUser GetAuthenticatedUser<T>(this T page) where T : RazorPage
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            return page.User.GetAuthenticatedUser();
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