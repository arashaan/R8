using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using System;

namespace R8.AspNetCore.Routing
{
    public static class CulturizedUrlHelperExtensions
    {
        /// <summary>
        /// Generates a URL with an absolute path for an action method.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <returns>The generated URL.</returns>
        public static string Action(this ICulturalizedUrlHelper helper)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.Action(
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
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="action">The name of the action method.</param>
        /// <returns>The generated URL.</returns>
        public static string Action(this ICulturalizedUrlHelper helper, string action)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.Action(action, controller: null, values: null, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name and route <paramref name="values"/>.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="action">The name of the action method.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public static string Action(this ICulturalizedUrlHelper helper, string action, object values)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.Action(action, controller: null, values: values, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> and <paramref name="controller"/> names.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <returns>The generated URL.</returns>
        public static string Action(this ICulturalizedUrlHelper helper, string action, string controller)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.Action(action, controller, values: null, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, and route <paramref name="values"/>.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public static string Action(this ICulturalizedUrlHelper helper, string action, string controller, object values)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.Action(action, controller, values, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, route <paramref name="values"/>, and
        /// <paramref name="protocol"/> to use. See the remarks section for important security information.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
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
        public static string Action(
            this ICulturalizedUrlHelper helper,
            string action,
            string controller,
            object values,
            string protocol)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.Action(action, controller, values, protocol, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, route <paramref name="values"/>,
        /// <paramref name="protocol"/> to use, and <paramref name="host"/> name.
        /// Generates an absolute URL if the <paramref name="protocol"/> and <paramref name="host"/> are
        /// non-<c>null</c>. See the remarks section for important security information.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
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
        public static string Action(
            this ICulturalizedUrlHelper helper,
            string action,
            string controller,
            object values,
            string protocol,
            string host)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.Action(action, controller, values, protocol, host, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, route <paramref name="values"/>,
        /// <paramref name="protocol"/> to use, <paramref name="host"/> name, and <paramref name="fragment"/>.
        /// Generates an absolute URL if the <paramref name="protocol"/> and <paramref name="host"/> are
        /// non-<c>null</c>. See the remarks section for important security information.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
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
        public static string Action(
            this ICulturalizedUrlHelper helper,
            string action,
            string controller,
            object values,
            string protocol,
            string host,
            string fragment)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            var culture = helper.ActionContext.HttpContext.RequestServices
                .GetService<IOptions<RequestLocalizationOptions>>().Value;
            values = CulturalizedUrlHelper.TryAddCulture(culture, helper.ActionContext.HttpContext.Request.RouteValues, values);
            return ((IUrlHelper)helper).Action(action, controller, values, protocol, host, fragment);
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified route <paramref name="values"/>.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public static string RouteUrl(this ICulturalizedUrlHelper helper, object values)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.RouteUrl(routeName: null, values: values, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="routeName"/>.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="routeName">The name of the route that is used to generate URL.</param>
        /// <returns>The generated URL.</returns>
        public static string RouteUrl(this ICulturalizedUrlHelper helper, string routeName)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.RouteUrl(routeName, values: null, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="routeName"/> and route
        /// <paramref name="values"/>.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="routeName">The name of the route that is used to generate URL.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public static string RouteUrl(this ICulturalizedUrlHelper helper, string routeName, object values)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.RouteUrl(routeName, values, protocol: null, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified route <paramref name="routeName"/> and route
        /// <paramref name="values"/>, which contains the specified <paramref name="protocol"/> to use. See the
        /// remarks section for important security information.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
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
        public static string RouteUrl(
            this ICulturalizedUrlHelper helper,
            string routeName,
            object values,
            string protocol)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.RouteUrl(routeName, values, protocol, host: null, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified route <paramref name="routeName"/> and route
        /// <paramref name="values"/>, which contains the specified <paramref name="protocol"/> to use and
        /// <paramref name="host"/> name. Generates an absolute URL if
        /// <see cref="UrlActionContext.Protocol"/> and <see cref="UrlActionContext.Host"/> are non-<c>null</c>.
        /// See the remarks section for important security information.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
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
        public static string RouteUrl(
            this ICulturalizedUrlHelper helper,
            string routeName,
            object values,
            string protocol,
            string host)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            return helper.RouteUrl(routeName, values, protocol, host, fragment: null);
        }

        /// <summary>
        /// Generates a URL with an absolute path for the specified route <paramref name="routeName"/> and route
        /// <paramref name="values"/>, which contains the specified <paramref name="protocol"/> to use,
        /// <paramref name="host"/> name and <paramref name="fragment"/>. Generates an absolute URL if
        /// <see cref="UrlActionContext.Protocol"/> and <see cref="UrlActionContext.Host"/> are non-<c>null</c>.
        /// See the remarks section for important security information.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
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
        public static string RouteUrl(
            this ICulturalizedUrlHelper helper,
            string routeName,
            object values,
            string protocol,
            string host,
            string fragment)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            var culture = helper.ActionContext.HttpContext.RequestServices
                .GetService<IOptions<RequestLocalizationOptions>>().Value;
            values = CulturalizedUrlHelper.TryAddCulture(culture, helper.ActionContext.HttpContext.Request.RouteValues, values);
            return ((IUrlHelper)helper).RouteUrl(routeName, values, protocol, host, fragment);
        }

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <returns>The generated URL.</returns>
        public static string Page(this ICulturalizedUrlHelper urlHelper, string pageName)
            => Page(urlHelper, pageName, values: null);

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="pageHandler">The handler to generate the url for.</param>
        /// <returns>The generated URL.</returns>
        public static string Page(this ICulturalizedUrlHelper urlHelper, string pageName, string pageHandler)
            => Page(urlHelper, pageName, pageHandler, values: null);

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public static string Page(this ICulturalizedUrlHelper urlHelper, string pageName, object values)
            => Page(urlHelper, pageName, pageHandler: null, values: values);

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="pageHandler">The handler to generate the url for.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        public static string Page(
            this ICulturalizedUrlHelper urlHelper,
            string pageName,
            string pageHandler,
            object values)
            => Page(urlHelper, pageName, pageHandler, values, protocol: null);

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="pageName"/>. See the remarks section
        /// for important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="ICulturalizedUrlHelper"/>.</param>
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
        public static string Page(
            this ICulturalizedUrlHelper urlHelper,
            string pageName,
            string pageHandler,
            object values,
            string protocol)
            => Page(urlHelper, pageName, pageHandler, values, protocol, host: null, fragment: null);

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="pageName"/>. See the remarks section for
        /// important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="ICulturalizedUrlHelper"/>.</param>
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
        public static string Page(
            this ICulturalizedUrlHelper urlHelper,
            string pageName,
            string pageHandler,
            object values,
            string protocol,
            string host)
            => Page(urlHelper, pageName, pageHandler, values, protocol, host, fragment: null);

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="pageName"/>. See the remarks section for
        /// important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="ICulturalizedUrlHelper"/>.</param>
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
        public static string Page(
            this ICulturalizedUrlHelper urlHelper,
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

            var culture = urlHelper.ActionContext.HttpContext.RequestServices
                .GetService<IOptions<RequestLocalizationOptions>>().Value;
            values = CulturalizedUrlHelper.TryAddCulture(culture, urlHelper.ActionContext.HttpContext.Request.RouteValues, values);
            return ((IUrlHelper)urlHelper).Page(pageName, pageHandler, values, protocol, host, fragment);
        }

        /// <summary>
        /// Generates an absolute URL for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, route <paramref name="values"/>,
        /// <paramref name="protocol"/> to use, <paramref name="host"/> name, and <paramref name="fragment"/>.
        /// Generates an absolute URL if the <paramref name="protocol"/> and <paramref name="host"/> are
        /// non-<c>null</c>. See the remarks section for important security information.
        /// </summary>
        /// <param name="helper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="action">The name of the action method. When <see langword="null" />, defaults to the current executing action.</param>
        /// <param name="controller">The name of the controller. When <see langword="null" />, defaults to the current executing controller.</param>
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
        public static string ActionLink(
            this ICulturalizedUrlHelper helper,
            string action = null,
            string controller = null,
            object values = null,
            string protocol = null,
            string host = null,
            string fragment = null)
        {
            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            var httpContext = helper.ActionContext.HttpContext;

            if (protocol == null)
            {
                protocol = httpContext.Request.Scheme;
            }

            if (host == null)
            {
                host = httpContext.Request.Host.ToUriComponent();
            }

            return Action(helper, action, controller, values, protocol, host, fragment);
        }

        /// <summary>
        /// Generates an absolute URL for a page, which contains the specified
        /// <paramref name="pageName"/>, <paramref name="pageHandler"/>, route <paramref name="values"/>,
        /// <paramref name="protocol"/> to use, <paramref name="host"/> name, and <paramref name="fragment"/>.
        /// Generates an absolute URL if the <paramref name="protocol"/> and <paramref name="host"/> are
        /// non-<c>null</c>. See the remarks section for important security information.
        /// </summary>
        /// <param name="urlHelper">The <see cref="ICulturalizedUrlHelper"/>.</param>
        /// <param name="pageName">The page name to generate the url for. When <see langword="null"/>, defaults to the current executing page.</param>
        /// <param name="pageHandler">The handler to generate the url for. When <see langword="null"/>, defaults to the current executing handler.</param>
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
        public static string PageLink(
            this ICulturalizedUrlHelper urlHelper,
            string pageName = null,
            string pageHandler = null,
            object values = null,
            string protocol = null,
            string host = null,
            string fragment = null)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            var httpContext = urlHelper.ActionContext.HttpContext;

            if (protocol == null)
            {
                protocol = httpContext.Request.Scheme;
            }

            if (host == null)
            {
                host = httpContext.Request.Host.ToUriComponent();
            }

            return Page(urlHelper, pageName, pageHandler, values, protocol, host, fragment);
        }
    }
}