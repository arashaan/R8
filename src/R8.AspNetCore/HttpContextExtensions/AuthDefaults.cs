using Microsoft.AspNetCore.Authentication.Cookies;

namespace R8.AspNetCore.HttpContextExtensions
{
    public class AuthDefaults
    {
        public const string AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        public static string ExternalAuthenticationScheme = $"{AuthenticationScheme}.External";
    }
}