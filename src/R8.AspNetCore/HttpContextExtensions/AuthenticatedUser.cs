using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using NodaTime;

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
                var claim = _claims.Find(x => x.Type == ClaimTypes.NameIdentifier);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return Guid.Empty;

                var valid = Guid.TryParse(claim.Value, out var guid);
                return !valid ? Guid.Empty : guid;
            }
        }

        public string Username
        {
            get
            {
                var claim = _claims.Find(x => x.Type == ClaimTypes.Name);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return null;

                return claim.Value;
            }
        }

        public string FirstName
        {
            get
            {
                var claim = _claims.Find(x => x.Type == ClaimTypes.GivenName);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return null;

                return claim.Value;
            }
        }

        public string LastName
        {
            get
            {
                var claim = _claims.Find(x => x.Type == ClaimTypes.Surname);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return null;

                return claim.Value;
            }
        }

        public string Email
        {
            get
            {
                var claim = _claims.Find(x => x.Type == ClaimTypes.Email);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return null;

                return claim.Value;
            }
        }

        public T GetRole<T>()
        {
            var roleString = GetRole();
            try
            {
                if (typeof(T) == typeof(string))
                    return (T)roleString;

                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                    return (T)converter.ConvertFromString(roleString.ToString());

                throw new ArgumentException($"Unable to cast role value to {typeof(T)}.");
            }
            catch (NotSupportedException)
            {
                throw new ArgumentException($"Unable to cast role value to {typeof(T)}.");
            }
        }

        public object GetRole()
        {
            var claim = _claims.Find(x => x.Type == ClaimTypes.Role);
            if (claim == null || string.IsNullOrEmpty(claim.Value))
                throw new NullReferenceException($"Unable to find role claim.");

            var roleString = claim.Value;
            return roleString;
        }

        public DateTimeZone TimeZone
        {
            get
            {
                var claim = _claims.Find(x => x.Type == "TimeZone");
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return DateTimeZoneProviders.Tzdb.GetSystemDefault();

                return DateTimeZoneProviders.Tzdb[claim.Value];
            }
        }

        public void AddClaim(Claim claim)
        {
            _claims ??= new List<Claim>();
            _claims.Add(claim);
        }

        public string GetClaim(string name)
        {
            if (_claims == null || !_claims.Any())
                throw new NullReferenceException($"List of claims is empty.");

            var claim = _claims.Find(x => x.Type == name);
            if (claim == null)
                throw new NullReferenceException($"Unable to find {name}");

            return claim.Value;
        }

        public T GetClaim<T>(string name) where T : class
        {
            var claim = GetClaim(name);
            return (T)(object)claim;
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