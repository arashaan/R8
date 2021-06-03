using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

using R8.AspNetCore.HttpContextExtensions;

using System;
using System.Linq;

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="page"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static RequestLocalizationOptions GetLocalization(this Microsoft.AspNetCore.Mvc.RazorPages.PageModel page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));
            var service = page.HttpContext.RequestServices.GetService(typeof(IOptions<RequestLocalizationOptions>));
            return ((IOptions<RequestLocalizationOptions>)service)?.Value;
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Microsoft.AspNetCore.Mvc.RazorPages.PageModel page)
        {
            var pageName = page.GetType().GetPagePath();
            var thisRoutes = page.Request.RouteValues;
            var values = page.GetLocalization()?.TryAddCulture(thisRoutes, new RouteValueDictionary());
            if (values == null || !values.Any())
                values = thisRoutes;

            return page.RedirectToPage(pageName, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Microsoft.AspNetCore.Mvc.RazorPages.PageModel page, object routeValues)
        {
            var pageName = page.GetType().GetPagePath();
            var thisRoutes = page.Request.RouteValues;
            var values = page.GetLocalization()?.TryAddCulture(thisRoutes, routeValues);
            if (values == null || !values.Any())
                values = thisRoutes;

            return page.RedirectToPage(pageName, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized<TPage>(this Microsoft.AspNetCore.Mvc.RazorPages.PageModel page) where TPage : PageModel
        {
            var pageName = typeof(TPage).GetPagePath();
            var thisRoutes = page.Request.RouteValues;
            var values = page.GetLocalization()?.TryAddCulture(thisRoutes, new RouteValueDictionary());
            if (values == null || !values.Any())
                values = thisRoutes;

            return page.RedirectToPage(pageName, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized<TPage>(this Microsoft.AspNetCore.Mvc.RazorPages.PageModel page, object routeValues) where TPage : PageModel
        {
            var pageName = typeof(TPage).GetPagePath();
            var thisRoutes = page.Request.RouteValues;
            var values = page.GetLocalization()?.TryAddCulture(thisRoutes, routeValues);
            if (values == null || !values.Any())
                values = thisRoutes;

            return page.RedirectToPage(pageName, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Microsoft.AspNetCore.Mvc.RazorPages.PageModel page, string pageName)
        {
            var thisRoutes = page.Request.RouteValues;
            var values = page.GetLocalization()?.TryAddCulture(thisRoutes, new RouteValueDictionary());
            if (values == null || !values.Any())
                values = thisRoutes;

            return page.RedirectToPage(pageName, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Microsoft.AspNetCore.Mvc.RazorPages.PageModel page, string pageName, object routeValues)
        {
            var thisRoutes = page.Request.RouteValues;
            var values = page.GetLocalization()?.TryAddCulture(thisRoutes, routeValues);
            if (values == null || !values.Any())
                values = thisRoutes;

            return page.RedirectToPage(pageName, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Microsoft.AspNetCore.Mvc.RazorPages.PageModel page, string pageName, string pageHandler)
        {
            var thisRoutes = page.Request.RouteValues;
            var values = page.GetLocalization()?.TryAddCulture(thisRoutes, new RouteValueDictionary());
            if (values == null || !values.Any())
                values = thisRoutes;

            return page.RedirectToPage(pageName, pageHandler, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Microsoft.AspNetCore.Mvc.RazorPages.PageModel page, string pageName, string pageHandler, object routeValues)
        {
            var thisRoutes = page.Request.RouteValues;
            var values = page.GetLocalization()?.TryAddCulture(thisRoutes, routeValues);
            if (values == null || !values.Any())
                values = thisRoutes;

            return page.RedirectToPage(pageName, pageHandler, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Microsoft.AspNetCore.Mvc.RazorPages.PageModel page, string pageName, string pageHandler, object routeValues, string fragment)
        {
            var thisRoutes = page.Request.RouteValues;
            var values = page.GetLocalization()?.TryAddCulture(thisRoutes, routeValues);
            if (values == null || !values.Any())
                values = thisRoutes;

            return page.RedirectToPage(pageName, pageHandler, values, fragment);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Microsoft.AspNetCore.Mvc.RazorPages.PageModel page, string pageName, string pageHandler, string fragment)
        {
            var thisRoutes = page.Request.RouteValues;
            var values = page.GetLocalization()?.TryAddCulture(thisRoutes, new RouteValueDictionary());
            if (values == null || !values.Any())
                values = thisRoutes;

            return page.RedirectToPage(pageName, pageHandler, values, fragment);
        }
    }
}