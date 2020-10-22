using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using R8.Lib.AspNetCore.Base;

using System;

namespace R8.Lib.AspNetCore.Routing
{
    public class CulturalizedUrlHelper : ICulturalizedUrlHelper
    {
        private IOptions<RequestLocalizationOptions> Culture =>
            ActionContext.HttpContext.RequestServices.GetService(typeof(IOptions<RequestLocalizationOptions>)) as
                IOptions<RequestLocalizationOptions>;

        private readonly IUrlHelper _urlHelper;

        public CulturalizedUrlHelper(IActionContextAccessor actionContext, IUrlHelperFactory urlHelperFactory)
        {
            ActionContext = actionContext.ActionContext;
            _urlHelper = urlHelperFactory.GetUrlHelper(ActionContext);
        }

        public string Page<TPage>(bool endpointRoute = false) where TPage : PageModel
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

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <returns>The generated URL.</returns>
        public string Page(string pageName)
            => Page(pageName, values: null);

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="pageHandler">The handler to generate the url for.</param>
        /// <returns>The generated URL.</returns>
        public string Page(string pageName, string pageHandler)
            => Page(pageName, pageHandler, values: null);

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public string Page(string pageName, object values)
            => Page(pageName, pageHandler: null, values: values);

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="pageHandler">The handler to generate the url for.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public string Page(
            string pageName,
            string pageHandler,
            object values)
            => Page(pageName, pageHandler, values, protocol: null);

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="pageName"/>. See the remarks section
        /// for important security information.
        /// </summary>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="pageHandler">The handler to generate the url for.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <returns>The generated URL.</returns>
        /// <remarks>
        /// <para>
        /// This method uses the value of <see cref="Host"/> to populate the host section of the generated URI.
        /// Relying on the value of the current request can allow untrusted input to influence the resulting URI unless
        /// the <c>Host</c> header has been validated. See the deployment documentation for instructions on how to properly
        /// validate the <c>Host</c> header in your deployment environment.
        /// </para>
        /// </remarks>
        public string Page(
            string pageName,
            string pageHandler,
            object values,
            string protocol)
            => Page(pageName, pageHandler, values, protocol, host: null, fragment: null);

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="pageName"/>. See the remarks section for
        /// important security information.
        /// </summary>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="pageHandler">The handler to generate the url for.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="host">The host name for the URL.</param>
        /// <returns>The generated URL.</returns>
        /// <remarks>
        /// <para>
        /// The value of <paramref name="host"/> should be a trusted value. Relying on the value of the current request
        /// can allow untrusted input to influence the resulting URI unless the <c>Host</c> header has been validated.
        /// See the deployment documentation for instructions on how to properly validate the <c>Host</c> header in
        /// your deployment environment.
        /// </para>
        /// </remarks>
        public string Page(
            string pageName,
            string pageHandler,
            object values,
            string protocol,
            string host)
            => Page(pageName, pageHandler, values, protocol, host, fragment: null);

        public static object TryAddCulture(IOptions<RequestLocalizationOptions> cultureProvider, RouteValueDictionary requestRoutes, object values)
        {
            if (cultureProvider == null)
                throw new ArgumentNullException(nameof(cultureProvider));
            if (requestRoutes == null)
                throw new ArgumentNullException(nameof(requestRoutes));

            var routeValues = new RouteValueDictionary(values);
            var hasCultureRoute = requestRoutes.ContainsKey(LanguageRouteConstraint.Key);
            if (!hasCultureRoute)
                return routeValues;

            var currentCulture = requestRoutes[LanguageRouteConstraint.Key].ToString();
            var defaultCulture = cultureProvider.Value.DefaultRequestCulture.Culture.Name;
            if (routeValues.ContainsKey(LanguageRouteConstraint.Key))
                return routeValues;

            if (currentCulture != defaultCulture)
                routeValues[LanguageRouteConstraint.Key] = currentCulture;
            return routeValues;
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="pageName"/>. See the remarks section for
        /// important security information.
        /// </summary>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="pageHandler">The handler to generate the url for.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="host">The host name for the URL.</param>
        /// <param name="fragment">The fragment for the URL.</param>
        /// <returns>The generated URL.</returns>
        /// <remarks>
        /// <para>
        /// The value of <paramref name="host"/> should be a trusted value. Relying on the value of the current request
        /// can allow untrusted input to influence the resulting URI unless the <c>Host</c> header has been validated.
        /// See the deployment documentation for instructions on how to properly validate the <c>Host</c> header in
        /// your deployment environment.
        /// </para>
        /// </remarks>
        public string Page(
            string pageName,
            string pageHandler,
            object values,
            string protocol,
            string host,
            string fragment)
        {
            values = TryAddCulture(Culture, _urlHelper.ActionContext.HttpContext.Request.RouteValues, values);
            var page = _urlHelper.Page(pageName, pageHandler, values, protocol, host, fragment);
            return page;
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method.
        /// </summary>
        /// <returns>The generated URL.</returns>
        public string Action()
        {
            return _urlHelper.Action(
                action: null,
                controller: null,
                values: null,
                protocol: null,
                host: null,
                fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name.
        /// </summary>
        /// <param name="action">The name of the action method.</param>
        /// <returns>The generated URL.</returns>
        public string Action(string action)
        {
            return _urlHelper.Action(action, controller: null, values: null, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name and route <paramref name="values"/>.
        /// </summary>
        /// <param name="action">The name of the action method.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public string Action(string action, object values)
        {
            return _urlHelper.Action(action, controller: null, values: values, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> and <paramref name="controller"/> names.
        /// </summary>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <returns>The generated URL.</returns>
        public string Action(string action, string controller)
        {
            return _urlHelper.Action(action, controller, values: null, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, and route <paramref name="values"/>.
        /// </summary>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public string Action(string action, string controller, object values)
        {
            return _urlHelper.Action(action, controller, values, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, route <paramref name="values"/>, and
        /// <paramref name="protocol"/> to use. See the remarks section for important security information.
        /// </summary>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <returns>The generated URL.</returns>
        /// <remarks>
        /// <para>
        /// This method uses the value of <see cref="Host"/> to populate the host section of the generated URI.
        /// Relying on the value of the current request can allow untrusted input to influence the resulting URI unless
        /// the <c>Host</c> header has been validated. See the deployment documentation for instructions on how to properly
        /// validate the <c>Host</c> header in your deployment environment.
        /// </para>
        /// </remarks>
        public string Action(
            string action,
            string controller,
            object values,
            string protocol)
        {
            return _urlHelper.Action(action, controller, values, protocol, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, route <paramref name="values"/>,
        /// <paramref name="protocol"/> to use, and <paramref name="host"/> name.
        /// Generates an absolute URL if the <paramref name="protocol"/> and <paramref name="host"/> are
        /// non-<c>null</c>. See the remarks section for important security information.
        /// </summary>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="host">The host name for the URL.</param>
        /// <returns>The generated URL.</returns>
        /// <remarks>
        /// <para>
        /// The value of <paramref name="host"/> should be a trusted value. Relying on the value of the current request
        /// can allow untrusted input to influence the resulting URI unless the <c>Host</c> header has been validated.
        /// See the deployment documentation for instructions on how to properly validate the <c>Host</c> header in
        /// your deployment environment.
        /// </para>
        /// </remarks>
        public string Action(
            string action,
            string controller,
            object values,
            string protocol,
            string host)
        {
            return _urlHelper.Action(action, controller, values, protocol, host, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, route <paramref name="values"/>,
        /// <paramref name="protocol"/> to use, <paramref name="host"/> name, and <paramref name="fragment"/>.
        /// Generates an absolute URL if the <paramref name="protocol"/> and <paramref name="host"/> are
        /// non-<c>null</c>. See the remarks section for important security information.
        /// </summary>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="host">The host name for the URL.</param>
        /// <param name="fragment">The fragment for the URL.</param>
        /// <returns>The generated URL.</returns>
        /// <remarks>
        /// <para>
        /// The value of <paramref name="host"/> should be a trusted value. Relying on the value of the current request
        /// can allow untrusted input to influence the resulting URI unless the <c>Host</c> header has been validated.
        /// See the deployment documentation for instructions on how to properly validate the <c>Host</c> header in
        /// your deployment environment.
        /// </para>
        /// </remarks>
        public string Action(
            string action,
            string controller,
            object values,
            string protocol,
            string host,
            string fragment)
        {
            return _urlHelper.Action(new UrlActionContext()
            {
                Action = action,
                Controller = controller,
                Host = host,
                Values = values,
                Protocol = protocol,
                Fragment = fragment
            });
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