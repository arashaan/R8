using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using NodaTime;
using R8.Lib;
using R8.Lib.Enums;

namespace R8.AspNetCore
{
    public static class HttpContexts
    {
        public static IPAddress GetIpAddress(this HttpContext context)
        {
            var finalIp = context.Connection?.RemoteIpAddress;
            return finalIp;
        }

        public static string GetPageAbsolutePath(this HttpContext context)
        {
            return context.GetRouteData().Values["page"].ToString();
        }

        public static void GetOrAddTimeZone(this HttpContext context, DateTimeZone finalTimeZone)
        {
            var anonyTimeZoneId = context.Session.GetString(UserTimeZoneConstant);
            var hasSession = false;
            if (!string.IsNullOrEmpty(anonyTimeZoneId))
            {
                try
                {
                    var anonyTimeZone = DateTimeZoneProviders.Tzdb[anonyTimeZoneId];
                    hasSession = anonyTimeZone.Id == finalTimeZone.Id;
                }
                catch
                {
                }
            }

            if (!hasSession)
                context.Session.SetString(UserTimeZoneConstant, finalTimeZone.Id);
        }

        public static bool IsRightToLeft(this CultureInfo culture)
        {
            var rtl = culture.TextInfo.IsRightToLeft;
            return rtl;
        }

        public static Uri BuildUri(this HttpContext httpContext, string relativePath, Dictionary<string, string> queries = null)
        {
            var baseUrl = httpContext.GetBaseUrl();

            var queryBuilder = new QueryBuilder();
            if (queries != null)
                queryBuilder = new QueryBuilder(queries);

            var uri = new UriBuilder(baseUrl + relativePath);
            if (queryBuilder.Any())
            {
                uri.Query = queryBuilder.ToQueryString().ToString();
            }

            return uri.Uri;
        }

        public static string GetBaseUrl(this HttpContext httpContext, bool includeSchema = true)
        {
            var text = string.Empty;
            if (includeSchema)
                text += $"{httpContext.Request.Scheme}://";

            text += $"{httpContext.Request.Host}/";
            return text;
        }

        public static CurrentUser GetCurrentUser(this IEnumerable<Claim> claims)
        {
            if (claims?.Any() != true)
                return null;

            var currentUser = new CurrentUser();

            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (email != null)
                currentUser.Email = email.Value;

            var username = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            if (username != null)
                currentUser.Username = username.Value;

            var firstname = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
            if (firstname != null)
                currentUser.FirstName = firstname.Value;

            var lastname = claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
            if (lastname != null)
                currentUser.LastName = lastname.Value;

            var password = claims.FirstOrDefault(x => x.Type == ClaimTypes.Hash);
            if (password != null)
                currentUser.Password = password.Value;

            var timeZone = claims.FirstOrDefault(x => x.Type == "TimeZone");
            if (timeZone != null)
                currentUser.TimeZone = DateTimeZoneProviders.Tzdb[timeZone.Value];

            var id = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (id != null)
                currentUser.Id = id.Value;

            var role = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            currentUser.Role = role != null && !string.IsNullOrEmpty(role.Value)
                ? role.Value.ToEnum<Roles>()
                : Roles.User;

            return currentUser;
        }

        public const string UserTimeZoneConstant = "UserTimeZone";

        public static DateTimeZone GetTimeZone(this HttpContext httpContext, string fallBackId = "Turkey Standard Time")
        {
            DateTimeZone finalZone;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var user = httpContext.User.GetCurrentUser();
                finalZone = user.TimeZone;
            }
            else
            {
                var session = httpContext.Session.GetString(UserTimeZoneConstant);
                if (!string.IsNullOrEmpty(session))
                {
                    try
                    {
                        finalZone = DateTimeZoneProviders.Tzdb[session];
                    }
                    catch
                    {
                        finalZone = DateTimeZoneProviders.Tzdb[fallBackId];
                    }
                }
                else
                {
                    finalZone = DateTimeZoneProviders.Tzdb[fallBackId];
                }
            }

            return finalZone;
        }

        public static CurrentUser GetCurrentUser(this IPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(HttpContext));

            var connectedUser = ((ClaimsPrincipal)principal).Claims.GetCurrentUser();
            return connectedUser;
        }

        public static CurrentUser GetCurrentUser(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(ClaimsPrincipal));

            var connectedUser = principal.Claims.GetCurrentUser();
            return connectedUser;
        }

        public static CurrentUser GetCurrentUser(this HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(HttpContext));

            if (!httpContext.User.Identity.IsAuthenticated)
                return null;

            var currentUser = httpContext.User.GetCurrentUser();
            return currentUser;
        }
    }
}