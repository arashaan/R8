using R8.AspNetCore.HttpContextExtensions;

using System.Collections.Generic;
using System.Security.Claims;

namespace R8.AspNetCore.Test.FakeObjects
{
    public class FakeAuthUser : AuthenticatedUser
    {
        public FakeAuthUser()
        {
        }

        public FakeAuthUser(IEnumerable<Claim> claims) : base(claims)
        {
        }

        public Roles Role
        {
            get
            {
                var valid = this.TryGetRole<Roles>(out var role);
                return valid ? role : Roles.User;
            }
        }

        public override string ToString()
        {
            return this.Role.ToString();
        }
    }
}