using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

using System;

namespace R8.AspNetCore.Routing
{
    public class CulturalizedUrlHelper : ICulturalizedUrlHelper
    {
        private readonly IUrlHelper _urlHelper;

        public CulturalizedUrlHelper(IActionContextAccessor actionContext, IUrlHelperFactory urlHelperFactory)
        {
            ActionContext = actionContext.ActionContext ?? throw new InvalidOperationException($"{typeof(IActionContextAccessor)} must been registered in dependencies.");
            _urlHelper = urlHelperFactory.GetUrlHelper(ActionContext);
        }

        public static object TryAddCulture(RequestLocalizationOptions cultureProvider, RouteValueDictionary requestRoutes, object values)
        {
            if (cultureProvider == null)
                throw new ArgumentNullException(nameof(cultureProvider));
            if (requestRoutes == null)
                throw new ArgumentNullException(nameof(requestRoutes));

            var routeValues = new RouteValueDictionary(values);
            var hasCultureRoute = requestRoutes.ContainsKey(R8.AspNetCore.Localization.Constraints.LanguageKey);
            if (!hasCultureRoute)
                return routeValues;

            var currentCulture = requestRoutes[R8.AspNetCore.Localization.Constraints.LanguageKey].ToString();
            var defaultCulture = cultureProvider.DefaultRequestCulture.Culture.Name;
            if (routeValues.ContainsKey(R8.AspNetCore.Localization.Constraints.LanguageKey))
                return routeValues;

            if (currentCulture != defaultCulture)
                routeValues[R8.AspNetCore.Localization.Constraints.LanguageKey] = currentCulture;
            return routeValues;
        }

        public string Page<TPage>(bool endpointRoute = false) where TPage : PageModelBase
        {
            var currentConfig = new PageHandlerConfiguration
            {
                EndWithIndex = true
            };
            var address = PageHandlers.GetPath(typeof(TPage), currentConfig);
            return endpointRoute
                ? this.Page(address)
                : address;
        }

        public string Action(UrlActionContext actionContext)
        {
            return _urlHelper.Action(actionContext);
        }

        public string Content(string contentPath)
        {
            return _urlHelper.Content(contentPath);
        }

        public bool IsLocalUrl(string url)
        {
            return _urlHelper.IsLocalUrl(url);
        }

        public string Link(string routeName, object values)
        {
            return _urlHelper.Link(routeName, values);
        }

        public string RouteUrl(UrlRouteContext routeContext)
        {
            return _urlHelper.RouteUrl(routeContext);
        }

        public ActionContext ActionContext { get; }
    }
}