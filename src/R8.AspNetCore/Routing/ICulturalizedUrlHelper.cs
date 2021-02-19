using Microsoft.AspNetCore.Mvc;

namespace R8.AspNetCore.Routing
{
    /// <summary>
    /// Defined the contract for the helper to build URLs for ASP.NET MVC within an application based on <see cref="IUrlHelper"/> interface with respect to <c>culture</c> route.
    /// </summary>
    public interface ICulturalizedUrlHelper : IUrlHelper
    {
        /// <summary>
        /// Generates a URL with a relative path for the specified <see cref="TPage"></see>.
        /// </summary>
        /// <param name="endpointRoute"></param>
        /// <returns>The generated URL.</returns>
        string Page<TPage>(bool endpointRoute = false) where TPage : PageModelBase;
    }
}