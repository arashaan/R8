using System.Globalization;

namespace R8.Lib
{
    public static class Extensions
    {
        /// <summary>
        /// Returns base url in Development
        /// </summary>
        /// <param name="culture"></param>
        public static string GetTwoLetterCulture(this CultureInfo culture)
        {
            var result = culture.TwoLetterISOLanguageName;
            return result;
        }
    }
}