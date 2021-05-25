using Microsoft.IdentityModel.Tokens;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace R8.AspNetCore
{
    public static class JwtAuthentication
    {
        /// <summary>
        /// Generates token based on HMAC-SHA256 encryption for given <see cref="ClaimsIdentity"/> object.
        /// </summary>
        /// <param name="claimsIdentity"></param>
        /// <param name="key"></param>
        /// <param name="expiryDateTime"></param>
        /// <param name="securityToken"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SecurityTokenEncryptionFailedException"></exception>
        /// <returns>A token.</returns>
        public static string GenerateAccessToken(this ClaimsIdentity claimsIdentity, byte[] key, DateTime expiryDateTime, out JwtSecurityToken securityToken)
        {
            if (claimsIdentity == null) throw new ArgumentNullException(nameof(claimsIdentity));
            if (key == null) throw new ArgumentNullException(nameof(key));

            securityToken = new JwtSecurityToken(
                claims: claimsIdentity.Claims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                notBefore: DateTime.UtcNow,
                expires: expiryDateTime);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        /// <summary>
        /// Returns a <see cref="ClaimsIdentity"/> based on given token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="key"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ClaimsIdentity ClaimIdentityByAccessToken(string token, byte[] key)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (key == null) throw new ArgumentNullException(nameof(key));

            var handler = new JwtSecurityTokenHandler();
            if (!(handler.ReadToken(token) is JwtSecurityToken))
                return null;

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

            if (!(claimsPrincipal?.Identity is ClaimsIdentity claimsIdentity) || !claimsIdentity.Claims.Any())
                return null;

            return claimsIdentity;
        }
    }
}