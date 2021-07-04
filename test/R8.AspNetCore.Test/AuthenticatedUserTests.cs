using NodaTime;

using R8.AspNetCore.Test.FakeObjects;

using System;
using System.Collections.Generic;
using System.Security.Claims;

using Xunit;

namespace R8.AspNetCore.Test
{
    public class AuthenticatedUserTests
    {
        [Fact]
        public void CallAuthenticatedUser()
        {
            // Assets
            var claims = new List<Claim>();
            const string id = "303c7f3a-1324-4f91-9f6a-087a32773e9b";
            claims.Add(new Claim(ClaimTypes.NameIdentifier, id));
            claims.Add(new Claim(ClaimTypes.Name, "iamr8"));
            claims.Add(new Claim(ClaimTypes.GivenName, "Arash"));
            claims.Add(new Claim(ClaimTypes.Surname, "Shabbeh"));
            claims.Add(new Claim(ClaimTypes.Email, "arash.shabbeh@gmail.com"));
            claims.Add(new Claim(ClaimTypes.Role, nameof(Roles.SysUser)));
            claims.Add(new Claim("TimeZone", DateTimeZone.Utc.Id));

            // Act
            var authenticatedUser = new FakeAuthUser(claims);

            // Arrange
            Assert.NotNull(authenticatedUser);
            Assert.Equal("Arash", authenticatedUser.FirstName);
            Assert.Equal("Shabbeh", authenticatedUser.LastName);
            Assert.Equal(Guid.Parse(id), authenticatedUser.Id);
            Assert.Equal("iamr8", authenticatedUser.Username);
            Assert.Equal("arash.shabbeh@gmail.com", authenticatedUser.Email);
            Assert.Equal(Roles.SysUser, authenticatedUser.Role);
        }

        [Fact]
        public void CallAuthenticatedUser_RoleNull()
        {
            // Assets
            var claims = new List<Claim>();
            const string id = "303c7f3a-1324-4f91-9f6a-087a32773e9b";
            claims.Add(new Claim(ClaimTypes.NameIdentifier, id));
            claims.Add(new Claim(ClaimTypes.Name, "iamr8"));
            claims.Add(new Claim(ClaimTypes.GivenName, "Arash"));
            claims.Add(new Claim(ClaimTypes.Surname, "Shabbeh"));
            claims.Add(new Claim(ClaimTypes.Email, "arash.shabbeh@gmail.com"));
            claims.Add(new Claim("TimeZone", DateTimeZone.Utc.Id));

            // Act
            var authenticatedUser = new FakeAuthUser(claims);

            // Arrange
            Assert.NotNull(authenticatedUser);
            Assert.Equal("Arash", authenticatedUser.FirstName);
            Assert.Equal("Shabbeh", authenticatedUser.LastName);
            Assert.Equal(Guid.Parse(id), authenticatedUser.Id);
            Assert.Equal("iamr8", authenticatedUser.Username);
            Assert.Equal("arash.shabbeh@gmail.com", authenticatedUser.Email);
            Assert.Equal(Roles.User, authenticatedUser.Role);
        }
    }
}