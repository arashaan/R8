using Microsoft.AspNetCore.Authentication.Cookies;

namespace R8.AspNetCore
{
    public class AuthDefaults
    {
        public const string AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        public static string ExternalAuthenticationScheme = $"{AuthenticationScheme}.External";
    }
}