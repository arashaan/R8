using System;
using System.Globalization;

namespace R8.Lib
{
    public static class Extensions
    {
        /// <summary>
        /// Returns an ISO Language name for given culture.
        /// </summary>
        /// <param name="culture"><see cref="CultureInfo"/> to get ISO Name</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="string"/> value.</returns>
        public static string GetTwoLetterCulture(this CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));

            return culture.TwoLetterISOLanguageName;
        }
    }
}