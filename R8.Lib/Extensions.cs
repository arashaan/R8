using System;
using System.Globalization;

namespace R8.Lib
{
    public static class Extensions
    {
        /// <summary>
        /// Returns Culture's Two Letter ISO Language Name ( such as en )
        /// </summary>
        /// <param name="culture"><see cref="CultureInfo"/> to get ISO Name</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetTwoLetterCulture(this CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));

            var result = culture.TwoLetterISOLanguageName;
            return result;
        }
    }
}