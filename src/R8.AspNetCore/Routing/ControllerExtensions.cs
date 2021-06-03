using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

using System;
using System.Linq;

namespace R8.AspNetCore.Routing
{
    public static class ControllerExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="controller"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static RequestLocalizationOptions GetLocalization(this Microsoft.AspNetCore.Mvc.Controller controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            var service = controller.HttpContext.RequestServices.GetService(typeof(IOptions<RequestLocalizationOptions>));
            return ((IOptions<RequestLocalizationOptions>)service)?.Value;
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Controller controller, string pageName, string pageHandler)
        {
            var thisRoutes = controller.Request.RouteValues;
            var values = controller.GetLocalization()?.TryAddCulture(thisRoutes, new RouteValueDictionary());
            if (values == null || !values.Any())
                values = thisRoutes;

            return controller.RedirectToPage(pageName, pageHandler, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Controller controller, string pageName, string pageHandler, object routeValues)
        {
            var thisRoutes = controller.Request.RouteValues;
            var values = controller.GetLocalization()?.TryAddCulture(thisRoutes, routeValues);
            if (values == null || !values.Any())
                values = thisRoutes;

            return controller.RedirectToPage(pageName, pageHandler, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Controller controller, string pageName, string pageHandler, object routeValues, string fragment)
        {
            var thisRoutes = controller.Request.RouteValues;
            var values = controller.GetLocalization()?.TryAddCulture(thisRoutes, routeValues);
            if (values == null || !values.Any())
                values = thisRoutes;

            return controller.RedirectToPage(pageName, pageHandler, values, fragment);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Controller controller, string pageName, string pageHandler, string fragment)
        {
            var thisRoutes = controller.Request.RouteValues;
            var values = controller.GetLocalization()?.TryAddCulture(thisRoutes, new RouteValueDictionary());
            if (values == null || !values.Any())
                values = thisRoutes;

            return controller.RedirectToPage(pageName, pageHandler, values, fragment);
        }

        public static RedirectToPageResult RedirectToPageLocalized<TPage>(this Controller controller) where TPage : PageModel
        {
            var pageName = typeof(TPage).GetPagePath();
            var thisRoutes = controller.Request.RouteValues;
            var values = controller.GetLocalization()?.TryAddCulture(thisRoutes, new RouteValueDictionary());
            if (values == null || !values.Any())
                values = thisRoutes;

            return controller.RedirectToPage(pageName, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized<TPage>(this Controller controller, object routeValues) where TPage : PageModel
        {
            var pageName = typeof(TPage).GetPagePath();
            var thisRoutes = controller.Request.RouteValues;
            var values = controller.GetLocalization()?.TryAddCulture(thisRoutes, routeValues);
            if (values == null || !values.Any())
                values = thisRoutes;

            return controller.RedirectToPage(pageName, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Controller controller, string pageName)
        {
            var thisRoutes = controller.Request.RouteValues;
            var values = controller.GetLocalization()?.TryAddCulture(thisRoutes, new RouteValueDictionary());
            if (values == null || !values.Any())
                values = thisRoutes;

            return controller.RedirectToPage(pageName, values);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Controller controller, string pageName, object routeValues)
        {
            var thisRoutes = controller.Request.RouteValues;
            var values = controller.GetLocalization()?.TryAddCulture(thisRoutes, routeValues);
            if (values == null || !values.Any())
                values = thisRoutes;

            return controller.RedirectToPage(pageName, values);
        }
    }
}