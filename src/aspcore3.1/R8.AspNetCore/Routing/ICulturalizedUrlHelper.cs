using Microsoft.AspNetCore.Mvc;

namespace R8.AspNetCore.Routing
{
    public interface ICulturalizedUrlHelper : IUrlHelper
    {
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
        string Action(string action, string controller, object values, string protocol, string host, string fragment);

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
        string Action(string action, string controller, object values, string protocol, string host);

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
        /// This method uses the value of <see cref="HttpRequest.Host"/> to populate the host section of the generated URI.
        /// Relying on the value of the current request can allow untrusted input to influence the resulting URI unless
        /// the <c>Host</c> header has been validated. See the deployment documentation for instructions on how to properly
        /// validate the <c>Host</c> header in your deployment environment.
        /// </para>
        /// </remarks>
        string Action(string action, string controller, object values, string protocol);

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name, <paramref name="controller"/> name, and route <paramref name="values"/>.
        /// </summary>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        string Action(string action, string controller, object values);

        /// <summary>
        /// Generates a URL with an absolute path for an action method.
        /// </summary>
        /// <returns>The generated URL.</returns>
        string Action();

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name.
        /// </summary>
        /// <param name="action">The name of the action method.</param>
        /// <returns>The generated URL.</returns>
        string Action(string action);

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> name and route <paramref name="values"/>.
        /// </summary>
        /// <param name="action">The name of the action method.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        string Action(string action, object values);

        /// <summary>
        /// Generates a URL with an absolute path for an action method, which contains the specified
        /// <paramref name="action"/> and <paramref name="controller"/> names.
        /// </summary>
        /// <param name="action">The name of the action method.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <returns>The generated URL.</returns>
        string Action(string action, string controller);

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <returns>The generated URL.</returns>
        string Page(string pageName);

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="pageHandler">The handler to generate the url for.</param>
        /// <returns>The generated URL.</returns>
        string Page(string pageName, string pageHandler);

        string Page(string pageName, object values);

        /// <summary>
        /// Generates a URL with a relative path for the specified <paramref name="pageName"/>.
        /// </summary>
        /// <param name="pageName">The page name to generate the url for.</param>
        /// <param name="pageHandler">The handler to generate the url for.</param>
        /// <param name="values">An object that contains route values.</param>
        /// <returns>The generated URL.</returns>
        string Page(string pageName, string pageHandler, object values);

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
        /// This method uses the value of <see cref="HttpRequest.Host"/> to populate the host section of the generated URI.
        /// Relying on the value of the current request can allow untrusted input to influence the resulting URI unless
        /// the <c>Host</c> header has been validated. See the deployment documentation for instructions on how to properly
        /// validate the <c>Host</c> header in your deployment environment.
        /// </para>
        /// </remarks>
        string Page(string pageName, string pageHandler, object values, string protocol);

        string Page(string pageName, string pageHandler, object values, string protocol, string host);

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
        string Page(string pageName, string pageHandler, object values, string protocol, string host, string fragment);

        /// <summary>
        /// Generates a URL with a relative path for the specified <see cref="TPage"></see>.
        /// </summary>
        /// <param name="endpointRoute"></param>
        /// <returns>The generated URL.</returns>
        string Page<TPage>(bool endpointRoute = false) where TPage : PageModelBase;
    }
}