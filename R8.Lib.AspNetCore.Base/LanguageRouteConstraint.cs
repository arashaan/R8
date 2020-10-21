using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace R8.Lib.AspNetCore.Base
{
    public class LanguageRouteConstraint : IRouteConstraint
    {
        public const string Key = "culture";

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!values.ContainsKey(Key))
                return false;

            var lang = values[routeKey] as string;
            if (string.IsNullOrWhiteSpace(lang))
                return false;

            return lang == "tr" || lang == "en" || lang == "fa";
        }
    }
}