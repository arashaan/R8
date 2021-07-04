using NodaTime;

using System;
using System.Security.Claims;

namespace R8.AspNetCore.HttpContextExtensions
{
    public interface IAuthenticatedUser
    {
        /// <summary>
        /// Gets unique identifier of authenticated user.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets username of authenticated user.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets first name of authenticated user.
        /// </summary>
        string FirstName { get; }

        /// <summary>
        /// Gets last name of authenticated user.
        /// </summary>
        string LastName { get; }

        /// <summary>
        /// Gets email address of authenticated user.
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Gets time-zone of authenticated user.
        /// </summary>
        /// <remarks>If unset, returns current system time zone.</remarks>
        DateTimeZone TimeZone { get; }

        /// <summary>
        /// Represents user's name full name.
        /// </summary>
        /// <returns>An <see cref="string"/> value.</returns>
        string GetFullName();

        /// <summary>
        /// Returns user's role in given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>An <see cref="object"/> for user's role</returns>
        bool TryGetRole<T>(out T role);

        /// <summary>
        /// Adds claims to user's claims directory.
        /// </summary>
        /// <param name="claim">A <see cref="Claim"/> object that representing specific claim type in user's principals.</param>
        void AddClaim(Claim claim);

        /// <summary>
        /// Finds specified claim name from user's claims directory.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>A <see cref="string"/> value.</returns>
        bool TryGetClaim(string name, out string value);
    }
}