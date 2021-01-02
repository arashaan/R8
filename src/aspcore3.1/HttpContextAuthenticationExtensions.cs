using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace R8.AspNetCore
{
    public static class HttpContextAuthenticationExtensions
    {
        /// <summary>
        /// Signs in using given claims and according to specific authentication scheme.
        /// </summary>
        /// <param name="httpContext">An <see cref="HttpContext"/> object.</param>
        /// <param name="claims">A collection of <see cref="Claim"/> that representing users credential data.</param>
        /// <param name="isPersistent">You may want the cookie to persist across browser sessions. This persistence should only be enabled with explicit user consent with a "Remember Me" check box on sign in or a similar mechanism.</param>
        /// <param name="authResult">The <see cref="AuthenticationProperties"/> properties.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="Task{T}"/> object that representing asynchronous operation.</returns>
        /// <remarks>If returns true, user is fully signed in, and not signed in yet otherwise.</remarks>
        public static async Task<bool> SignInAsync(this HttpContext httpContext, List<Claim> claims, bool isPersistent,
            AuthenticateResult authResult = null)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            var claimsIdentity = new ClaimsIdentity(claims, AuthDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties
            {
                IssuedUtc = authResult?.Properties.IssuedUtc ?? DateTimeOffset.UtcNow,
                ExpiresUtc = authResult?.Properties.ExpiresUtc ?? DateTimeOffset.UtcNow.AddDays(30),
                IsPersistent = authResult?.Properties.IsPersistent ?? isPersistent
            };

            await httpContext.SignInAsync(AuthDefaults.AuthenticationScheme, claimsPrincipal, authProperties).ConfigureAwait(false);
            httpContext.User.AddIdentity(claimsIdentity);
            return httpContext.User.Identities?.Any(x =>
                x.IsAuthenticated &&
                x.AuthenticationType == AuthDefaults.AuthenticationScheme &&
                x.Name == claimsPrincipal.Identity.Name) ?? false;
        }
    }
}