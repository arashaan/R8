using Microsoft.AspNetCore.Mvc;

namespace R8.AspNetCore.Routing
{
    public static class ControllerExtensions
    {
        public static RedirectToPageResult RedirectToPageLocalized(this Controller controller, string pageName, string pageHandler) => controller.RedirectToPageLocalized(pageName, pageHandler, null);

        public static RedirectToPageResult RedirectToPageLocalized(this Controller controller, string pageName, string pageHandler, object routeValues) => controller.RedirectToPageLocalized(pageName, pageHandler, routeValues, null);

        public static RedirectToPageResult RedirectToPageLocalized(this Controller controller, string pageName, string pageHandler, object routeValues, string fragment)
        {
            var values = CultureExtensions.GetDictionary(
                localization: controller.HttpContext.GetLocalization(),
                requestRoute: controller.Request.RouteValues,
                newRouteValue: routeValues);
            return controller.RedirectToPage(pageName, pageHandler, values, fragment);
        }

        public static RedirectToPageResult RedirectToPageLocalized(this Controller controller, string pageName, string pageHandler, string fragment) => controller.RedirectToPageLocalized(pageName, pageHandler, null, fragment);

        public static RedirectToPageResult RedirectToPageLocalized(this Controller controller, string pageName) => controller.RedirectToPageLocalized(pageName, null);

        public static RedirectToPageResult RedirectToPageLocalized(this Controller controller, string pageName, object routeValues) => controller.RedirectToPageLocalized(pageName, null, routeValues);
    }
}