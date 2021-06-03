using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using System;

namespace R8.AspNetCore.Routing
{
    public static class CultureExtensions
    {
        /// <summary>
        /// Generates a URL with an absolute path for an action method.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <returns>The generated URL.</returns>
        public static string ActionLocalized(this IUrlHelper urlHelper)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            return urlHelper.ActionLocalized(
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
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="action">The name of the action method.</param>
        /// <returns>The generated URL.</returns>
        public static string ActionLocalized(this IUrlHelper urlHelper, string action)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            return urlHelper.ActionLocalized(action, controller: null, values: null, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name and route <paramref name="values"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="action">The name of the action method.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public static string ActionLocalized(this IUrlHelper urlHelper, string action, object values)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            return urlHelper.ActionLocalized(action, controller: null, values: values, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> and <paramref name="controller"/> names.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <returns>The generated URL.</returns>
        public static string ActionLocalized(this IUrlHelper urlHelper, string action, string controller)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            return urlHelper.ActionLocalized(action, controller, values: null, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, and route <paramref name="values"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public static string ActionLocalized(this IUrlHelper urlHelper, string action, string controller, object values)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            return urlHelper.ActionLocalized(action, controller, values, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, route <paramref name="values"/>, and
        /// <paramref name="protocol"/> to use. See the remarks section for important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
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
        public static string ActionLocalized(
            this IUrlHelper urlHelper,
            string action,
            string controller,
            object values,
            string protocol)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            return urlHelper.ActionLocalized(action, controller, values, protocol, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, route <paramref name="values"/>,
        /// <paramref name="protocol"/> to use, and <paramref name="host"/> name.
        /// Generates an absolute URL if the <paramref name="protocol"/> and <paramref name="host"/> are
        /// non-<c>null</c>. See the remarks section for important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
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
        public static string ActionLocalized(
            this IUrlHelper urlHelper,
            string action,
            string controller,
            object values,
            string protocol,
            string host)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            return urlHelper.ActionLocalized(action, controller, values, protocol, host, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, route <paramref name="values"/>,
        /// <paramref name="protocol"/> to use, <paramref name="host"/> name, and <paramref name="fragment"/>.
        /// Generates an absolute URL if the <paramref name="protocol"/> and <paramref name="host"/> are
        /// non-<c>null</c>. See the remarks section for important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
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
        public static string ActionLocalized(
            this IUrlHelper urlHelper,
            string action,
            string controller,
            object values,
            string protocol,
            string host,
            string fragment)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }
            var requestLocalizationOptions = urlHelper.ActionContext.HttpContext.RequestServices
                .GetService<IOptions<RequestLocalizationOptions>>();
            var requestLocalization = requestLocalizationOptions?.Value;
            if (requestLocalization != null)
                values = requestLocalization.TryAddCulture(urlHelper.ActionContext.HttpContext.Request.RouteValues,
                    values);

            return urlHelper.Action(action, controller, values, protocol, host, fragment);
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified route <paramref name="values"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public static string RouteUrlLocalized(this IUrlHelper urlHelper, object values)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            return urlHelper.RouteUrlLocalized(routeName: null, values: values, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="routeName"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="routeName">The name of the route that is used to generate URL.</param>
        /// <returns>The generated URL.</returns>
        public static string RouteUrlLocalized(this IUrlHelper urlHelper, string routeName)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            return urlHelper.RouteUrlLocalized(routeName, values: null, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="routeName"/> and route
        /// <paramref name="values"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="routeName">The name of the route that is used to generate URL.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public static string RouteUrlLocalized(this IUrlHelper urlHelper, string routeName, object values)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            return urlHelper.RouteUrlLocalized(routeName, values, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified route <paramref name="routeName"/> and route
        /// <paramref name="values"/>, which contains the specified <paramref name="protocol"/> to use. See the
        /// remarks section for important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="routeName">The name of the route that is used to generate URL.</param>
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
        public static string RouteUrlLocalized(
            this IUrlHelper urlHelper,
            string routeName,
            object values,
            string protocol)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            return urlHelper.RouteUrlLocalized(routeName, values, protocol, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified route <paramref name="routeName"/> and route
        /// <paramref name="values"/>, which contains the specified <paramref name="protocol"/> to use and
        /// <paramref name="host"/> name. Generates an absolute URL if
        /// <see cref="UrlActionContext.Protocol"/> and <see cref="UrlActionContext.Host"/> are non-<c>null</c>.
        /// See the remarks section for important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="routeName">The name of the route that is used to generate URL.</param>
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
        public static string RouteUrlLocalized(
            this IUrlHelper urlHelper,
            string routeName,
            object values,
            string protocol,
            string host)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            return urlHelper.RouteUrlLocalized(routeName, values, protocol, host, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified route <paramref name="routeName"/> and route
        /// <paramref name="values"/>, which contains the specified <paramref name="protocol"/> to use,
        /// <paramref name="host"/> name and <paramref name="fragment"/>. Generates an absolute URL if
        /// <see cref="UrlActionContext.Protocol"/> and <see cref="UrlActionContext.Host"/> are non-<c>null</c>.
        /// See the remarks section for important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="routeName">The name of the route that is used to generate URL.</param>
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
        public static string RouteUrlLocalized(
            this IUrlHelper urlHelper,
            string routeName,
            object values,
            string protocol,
            string host,
            string fragment)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            var requestLocalizationOptions = urlHelper.ActionContext.HttpContext.RequestServices
                .GetService<IOptions<RequestLocalizationOptions>>();
            var requestLocalization = requestLocalizationOptions?.Value;
            if (requestLocalization != null)
                values = requestLocalization.TryAddCulture(urlHelper.ActionContext.HttpContext.Request.RouteValues,
                    values);
            return urlHelper.RouteUrl(routeName, values, protocol, host, fragment);
        }

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <returns>The generated URL.</returns>
        public static string PageLocalized(this IUrlHelper urlHelper, string pageName)
            => PageLocalized(urlHelper, pageName, values: null);

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="pageHandler">The handler to generate the url for.</param>
        /// <returns>The generated URL.</returns>
        public static string PageLocalized(this IUrlHelper urlHelper, string pageName, string pageHandler)
            => PageLocalized(urlHelper, pageName, pageHandler, values: null);

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public static string PageLocalized(this IUrlHelper urlHelper, string pageName, object values)
            => PageLocalized(urlHelper, pageName, pageHandler: null, values: values);

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="pageHandler">The handler to generate the url for.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public static string PageLocalized(
            this IUrlHelper urlHelper,
            string pageName,
            string pageHandler,
            object values)
            => PageLocalized(urlHelper, pageName, pageHandler, values, protocol: null);

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="pageName"/>. See the remarks section
        /// for important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
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
        public static string PageLocalized(
            this IUrlHelper urlHelper,
            string pageName,
            string pageHandler,
            object values,
            string protocol)
            => PageLocalized(urlHelper, pageName, pageHandler, values, protocol, host: null, fragment: null);

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="pageName"/>. See the remarks section for
        /// important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
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
        public static string PageLocalized(
            this IUrlHelper urlHelper,
            string pageName,
            string pageHandler,
            object values,
            string protocol,
            string host)
            => PageLocalized(urlHelper, pageName, pageHandler, values, protocol, host, fragment: null);

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="pageName"/>. See the remarks section for
        /// important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
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
        public static string PageLocalized(
            this IUrlHelper urlHelper,
            string pageName,
            string pageHandler,
            object values,
            string protocol,
            string host,
            string fragment)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            var requestLocalizationOptions = urlHelper.ActionContext.HttpContext.RequestServices
                .GetService<IOptions<RequestLocalizationOptions>>();
            var requestLocalization = requestLocalizationOptions?.Value;
            if (requestLocalization != null)
                values = requestLocalization.TryAddCulture(urlHelper.ActionContext.HttpContext.Request.RouteValues,
                    values);

            return urlHelper.Page(pageName, pageHandler, values, protocol, host, fragment);
        }

        public static RouteValueDictionary TryAddCulture(this RequestLocalizationOptions cultureProvider, RouteValueDictionary requestRoutes, object values)
        {
            if (cultureProvider == null)
                return requestRoutes;
            if (requestRoutes == null)
                return new RouteValueDictionary();

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
    }
}