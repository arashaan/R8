using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;

using NodaTime;

namespace R8.AspNetCore.HttpContextExtensions
{
    public static class Extensions
    {
        /// <summary>
        /// Retrieves Absolute Path for current page according to <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">An <see cref="HttpContext"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="string"/> value that representing page's absolute path.</returns>
        /// <remarks>If <see cref="RouteData"/> doesn't have a key as <c>page</c>, result will be null.</remarks>
        public static string GetCurrentPageAbsolutePath(this HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.GetRouteData().Values["page"].ToString();
        }

        /// <summary>
        /// Retrieves specific <see cref="DateTimeZone"/> object according to current user time zone.
        /// </summary>
        /// <param name="context">An <see cref="HttpContext"/> object.</param>
        /// <param name="fallBackTimeZoneId">An <see cref="string"/> value that representing a valid time zone id, for using when we have errors in finding users time zone.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="DateTimeZone"/> object.</returns>
        public static DateTimeZone GetTimeZoneSession(this HttpContext context, string fallBackTimeZoneId = "Europe/Istanbul")
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            DateTimeZone finalZone;
            if (context.User.Identity.IsAuthenticated)
            {
                finalZone = context.User.GetAuthenticatedUser().TimeZone;
            }
            else
            {
                var session = context.Session.GetString(UserTimeZoneConstant);
                if (!string.IsNullOrEmpty(session))
                {
                    try
                    {
                        finalZone = DateTimeZoneProviders.Tzdb[session];
                    }
                    catch
                    {
                        finalZone = DateTimeZoneProviders.Tzdb[fallBackTimeZoneId];
                    }
                }
                else
                {
                    finalZone = DateTimeZoneProviders.Tzdb[fallBackTimeZoneId];
                }
            }

            return finalZone;
        }

        /// <summary>
        /// Stores given time-zone into <see cref="HttpContext"/> sessions.
        /// </summary>
        /// <param name="context">An <see cref="HttpContext"/> object.</param>
        /// <param name="timeZone">An <see cref="DateTimeZone"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void SetTimeZoneSession(this HttpContext context, DateTimeZone timeZone)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Session == null)
                throw new ArgumentNullException(nameof(context.Session));
            if (timeZone == null)
                throw new ArgumentNullException(nameof(timeZone));

            var session = context.Session.GetString(UserTimeZoneConstant);
            var hasSession = false;
            if (!string.IsNullOrEmpty(session))
            {
                try
                {
                    var anonyTimeZone = DateTimeZoneProviders.Tzdb[session];
                    hasSession = anonyTimeZone.Id == timeZone.Id;
                }
                catch
                {
                }
            }

            if (!hasSession)
                context.Session.SetString(UserTimeZoneConstant, timeZone.Id);
        }

        [Obsolete]
        public static bool IsRightToLeft(this CultureInfo culture)
        {
            return culture.TextInfo.IsRightToLeft;
        }

        /// <summary>
        /// Builds a <see cref="Uri"/> component based on given path and query strings.
        /// </summary>
        /// <param name="httpContext">A <see cref="HttpContext"/> object.</param>
        /// <param name="relativePath">An <see cref="string"/> that representing action endpoint path without query strings.</param>
        /// <param name="queries">A <see cref="Dictionary{TKey,TValue}"/> that representing query strings.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>An <see cref="Uri"/> component.</returns>
        public static Uri BuildUri(this HttpContext httpContext, string relativePath, Dictionary<string, string> queries = null)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var baseUrl = httpContext.GetBaseUrl();

            var queryBuilder = new QueryBuilder();
            if (queries != null)
                queryBuilder = new QueryBuilder(queries);

            var uri = new UriBuilder(baseUrl + relativePath);
            if (queryBuilder.Any())
                uri.Query = queryBuilder.ToQueryString().ToString();

            return uri.Uri;
        }

        /// <summary>
        /// Builds base url according to current <see cref="HttpContext"/> instance.
        /// </summary>
        /// <param name="httpContext">A <see cref="HttpContext"/> object.</param>
        /// <param name="includeScheme">A <see cref="bool"/> value that representing if scheme should be included.</param>
        /// <returns>An <see cref="string"/> value.</returns>
        public static string GetBaseUrl(this HttpContext httpContext, bool includeScheme = true)
        {
            var text = string.Empty;
            if (includeScheme)
                text += $"{httpContext.Request.Scheme}://";

            text += $"{httpContext.Request.Host}/";
            return text;
        }

        /// <summary>
        /// Returns a <see cref="AuthenticatedUser"/> object according to a collection of <see cref="Claim"/>.
        /// </summary>
        /// <param name="claims">A collection o <see cref="Claim"/> that representing authenticated user claims.</param>
        /// <returns>A <see cref="AuthenticatedUser"/> object.</returns>
        public static IAuthenticatedUser GetAuthenticatedUser(this IEnumerable<Claim> claims)
        {
            var claimsList = claims.ToList();
            if (claimsList?.Any() != true)
                return null;

            var currentUser = new AuthenticatedUser();
            currentUser.AddClaims(claimsList);
            // var dictionary = new Dictionary<string, object>();
            // foreach (var claim in claimsList)
            // {
            //     var key = claim.Type switch
            //     {
            //         ClaimTypes.Email => AuthenticatedUser.Key_Email,
            //         ClaimTypes.Name => AuthenticatedUser.Key_Username,
            //         ClaimTypes.GivenName => AuthenticatedUser.Key_FirstName,
            //         ClaimTypes.Surname => AuthenticatedUser.Key_LastName,
            //         ClaimTypes.
            //         _ => string.Empty
            //     };
            //
            //     dictionary.Add(key, claim.Value);
            // }
            //
            // var email = claimsList.Find(x => x.Type == ClaimTypes.Email);
            // if (email != null)
            //     currentUser.Email = email.Value;
            //
            // var username = claimsList.Find(x => x.Type == ClaimTypes.Name);
            // if (username != null)
            //     currentUser.Username = username.Value;
            //
            // var firstname = claimsList.Find(x => x.Type == ClaimTypes.GivenName);
            // if (firstname != null)
            //     currentUser.FirstName = firstname.Value;
            //
            // var lastname = claimsList.Find(x => x.Type == ClaimTypes.Surname);
            // if (lastname != null)
            //     currentUser.LastName = lastname.Value;
            //
            // var password = claimsList.Find(x => x.Type == ClaimTypes.Hash);
            // if (password != null)
            //     currentUser.Password = password.Value;
            //
            // var timeZone = claimsList.Find(x => x.Type == "TimeZone");
            // if (timeZone != null)
            //     currentUser.TimeZone = DateTimeZoneProviders.Tzdb[timeZone.Value];
            //
            // var id = claimsList.Find(x => x.Type == ClaimTypes.NameIdentifier);
            // if (id != null)
            //     currentUser.Id = id.Value;
            //
            // var role = claimsList.Find(x => x.Type == ClaimTypes.Role);
            // currentUser.Role = role != null && !string.IsNullOrEmpty(role.Value)
            //     ? role.Value.ToEnum<Roles>()
            //     : Roles.User;

            return currentUser;
        }

        private const string UserTimeZoneConstant = "UserTimeZone";

        /// <summary>
        /// Returns a <see cref="AuthenticatedUser"/> object according to an <see cref="IPrincipal"/> interface.
        /// </summary>
        /// <param name="principal">An <see cref="IPrincipal"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="AuthenticatedUser"/> object.</returns>
        public static IAuthenticatedUser GetAuthenticatedUser(this IPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(HttpContext));

            return ((ClaimsPrincipal)principal).Claims.GetAuthenticatedUser();
        }

        /// <summary>
        /// Returns a <see cref="AuthenticatedUser"/> object according to an a collection of claims that stored in <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="httpContext">An <see cref="HttpContext"/> object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="AuthenticatedUser"/> object.</returns>
        public static IAuthenticatedUser GetAuthenticatedUser(this HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(HttpContext));

            return !httpContext.User.Identity.IsAuthenticated
                ? null
                : httpContext.User.GetAuthenticatedUser();
        }
    }
}