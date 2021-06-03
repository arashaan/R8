using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;

using R8.AspNetCore.HttpContextExtensions;

using System;

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
        public static IAuthenticatedUser GetAuthenticatedUser<T>(this T page) where T : RazorPage => page.User.GetAuthenticatedUser();

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

        public static RedirectToPageResult RedirectToPageLocalized<TPage>(this TPage page) where TPage : PageModel => page.RedirectToPageLocalized(null);

        public static RedirectToPageResult RedirectToPageLocalized<TPage>(this TPage page, object routeValues) where TPage : PageModel => page.RedirectToPageLocalized(null, routeValues);

        public static RedirectToPageResult RedirectToPageLocalized<TPage>(this TPage page, string pageName) where TPage : PageModel => page.RedirectToPageLocalized(pageName, null);

        public static RedirectToPageResult RedirectToPageLocalized<TPage>(this TPage page, string pageName, object routeValues) where TPage : PageModel => page.RedirectToPageLocalized(pageName, null, routeValues);

        public static RedirectToPageResult RedirectToPageLocalized<TPage>(this TPage page, string pageName, string pageHandler) where TPage : PageModel => page.RedirectToPageLocalized(pageName, pageHandler, null);

        public static RedirectToPageResult RedirectToPageLocalized<TPage>(this TPage page, string pageName, string pageHandler, object routeValues) where TPage : PageModel => page.RedirectToPageLocalized(pageName, pageHandler, routeValues, null);

        public static RedirectToPageResult RedirectToPageLocalized<TPage>(this TPage page, string pageName, string pageHandler, object routeValues, string fragment) where TPage : PageModel
        {
            var thisRoutes = page.Request.RouteValues;
            var values = CultureExtensions.GetDictionary(
                localization: page.HttpContext.GetLocalization(),
                requestRoute: page.Request.RouteValues,
                newRouteValue: routeValues);
            return page.RedirectToPage(pageName, pageHandler, values, fragment);
        }

        public static RedirectToPageResult RedirectToPageLocalized<TPage>(this TPage page, string pageName, string pageHandler, string fragment) where TPage : PageModel => page.RedirectToPageLocalized(pageName, pageHandler, null, fragment);
    }
}