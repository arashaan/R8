using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace R8.Lib.AspNetCore
{
    public class AuthDefaults
    {
        public const string AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        public static string ExternalAuthenticationScheme = $"{AuthenticationScheme}.External";
    }

    public static class HttpContextAuthenticationExtensions
    {
        public static async Task<bool> SignInAsync(this HttpContext httpContext, List<Claim> claims, AuthenticateResult authResult = null)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            var claimsIdentity = new ClaimsIdentity(claims, AuthDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties
            {
                IssuedUtc = authResult?.Properties.IssuedUtc ?? DateTimeOffset.UtcNow,
                ExpiresUtc = authResult?.Properties.ExpiresUtc ?? DateTimeOffset.UtcNow.AddDays(30),
                IsPersistent = authResult?.Properties.IsPersistent ?? true
            };

            await httpContext.SignInAsync(AuthDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

            // if (authResult != null)
            //     await httpContext.SignOutAsync(authResult.Ticket.AuthenticationScheme, authResult.Properties);

            httpContext.User.AddIdentity(claimsIdentity);
            var isAuthenticated = httpContext.User.Identities?.Any(x =>
                x.IsAuthenticated &&
                x.AuthenticationType == AuthDefaults.AuthenticationScheme &&
                x.Name == claimsPrincipal.Identity.Name) ?? false;
            return isAuthenticated;
        }
    }
}