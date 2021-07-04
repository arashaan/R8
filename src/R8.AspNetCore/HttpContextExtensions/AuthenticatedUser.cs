using NodaTime;

using R8.Lib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace R8.AspNetCore.HttpContextExtensions
{
    public class AuthenticatedUser : IAuthenticatedUser
    {
        private List<Claim> _claims = new List<Claim>();

        public AuthenticatedUser()
        {
        }

        public AuthenticatedUser(IEnumerable<Claim> claims) : this()
        {
            AddClaims(claims);
        }

        internal void AddClaims(IEnumerable<Claim> claims)
        {
            _claims.AddRange(claims);
        }

        public Guid Id
        {
            get
            {
                try
                {
                    return TryGetClaim(ClaimTypes.NameIdentifier, out var claim)
                        ? Guid.TryParse(claim, out var guid)
                            ? guid
                            : Guid.Empty
                        : Guid.Empty;
                }
                catch (Exception e)
                {
                    return Guid.Empty;
                }
            }
        }

        public string Username
        {
            get
            {
                try
                {
                    return TryGetClaim(ClaimTypes.Name, out var claim)
                        ? claim
                        : null;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public string FirstName
        {
            get
            {
                try
                {
                    return TryGetClaim(ClaimTypes.GivenName, out var claim)
                        ? claim
                        : null;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public string LastName
        {
            get
            {
                try
                {
                    return TryGetClaim(ClaimTypes.Surname, out var claim)
                        ? claim
                        : null;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public string Email
        {
            get
            {
                try
                {
                    return TryGetClaim(ClaimTypes.Email, out var claim)
                        ? claim
                        : null;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public bool TryGetRole<T>(out T role)
        {
            try
            {
                var expectedType = typeof(T);
                var valid = TryGetClaim(ClaimTypes.Role, out var str);
                if (!valid)
                {
                    role = expectedType.IsValueType
                        ? (T)Activator.CreateInstance(expectedType)
                        : (T)(object)null;
                    return false;
                }

                if (expectedType == typeof(string))
                {
                    role = (T)(object)str;
                    return true;
                }

                var parsed = expectedType.TryParse(str, out var value);
                role = value == null
                    ? expectedType.IsValueType
                        ? (T)Activator.CreateInstance(expectedType)
                        : (T)(object)null
                    : (T)value;
                return parsed;
            }
            catch (NotSupportedException)
            {
                role = default;
                return false;
            }
        }

        public DateTimeZone TimeZone
        {
            get
            {
                try
                {
                    return TryGetClaim("TimeZone", out var claim)
                        ? DateTimeZoneProviders.Tzdb[claim]
                        : DateTimeZoneProviders.Tzdb.GetSystemDefault();
                }
                catch (Exception e)
                {
                    return DateTimeZoneProviders.Tzdb.GetSystemDefault();
                }
            }
        }

        public void AddClaim(Claim claim)
        {
            _claims ??= new List<Claim>();
            _claims.Add(claim);
        }

        public bool TryGetClaim(string name, out string value)
        {
            try
            {
                if (_claims?.Any() == true)
                {
                    var claim = _claims.FirstOrDefault(x => x.Type.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                    if (claim == null)
                    {
                        value = null;
                        return false;
                    }

                    value = claim.Value;
                    return true;
                }

                value = null;
                return false;
            }
            catch (Exception e)
            {
                value = null;
                return false;
            }
        }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        public override string ToString()
        {
            return Username;
        }
    }
}