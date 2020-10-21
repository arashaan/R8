using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace R8.Lib.AspNetCore.Base
{
    public static class Tokenizer
    {
        public static string GenerateAccessToken(this ClaimsIdentity claimsIdentity, byte[] key)
        {
            if (claimsIdentity == null) return null;

            var securityToken = new JwtSecurityToken(
                claims: claimsIdentity.Claims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                notBefore: DateTime.UtcNow,
                expires: DateTime.Now.AddYears(1));

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public static ClaimsIdentity ClaimIdentityByAccessToken(string token, byte[] key)
        {
            if (string.IsNullOrEmpty(token)) return null;

            var handler = new JwtSecurityTokenHandler();
            if (!(handler.ReadToken(token) is JwtSecurityToken)) return null;

            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateActor = false,
                ValidateIssuerSigningKey = true,
            };

            var claimsPrincipal = handler.ValidateToken(token, validationParameters, out _);
            if (claimsPrincipal == null) return null;

            if (!(claimsPrincipal.Identity is ClaimsIdentity claimsIdentity) || !claimsIdentity.Claims.Any())
                return null;

            return claimsIdentity;
        }
    }
}