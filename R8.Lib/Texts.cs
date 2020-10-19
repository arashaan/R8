using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace R8.Lib
{
    public static class Texts
    {
        private static readonly HashSet<char> DefaultNonWordCharacters = new HashSet<char> { ',', '.', ':', ';' };

        private static bool IsWhitespace(this char character)
        {
            return character == ' ' || character == 'n' || character == 't';
        }

        private static string ApplyModeratePersianRules(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (!text.ContainsFarsi())
                return text;

            return text
                .ApplyPersianYeKe()
                .ApplyHalfSpaceRule()
                .YeHeHalfSpace()
                .CleanupZwnj()
                .CleanupExtraMarks();
        }

        /// <summary>
        /// camelCase
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string word, bool ignoreSpaces = true, CultureInfo culture = null)
        {
            var currentCulture = culture ?? CultureInfo.CurrentCulture;

            var result = word.ToNormalized(culture: currentCulture);
            result = result.Substring(0, 1).ToLower(currentCulture) + result.Substring(1);
            if (ignoreSpaces)
                result = result.Replace(" ", string.Empty, true, currentCulture);

            return result;
        }

        /// <summary>
        /// Kebab Case
        /// </summary>
        /// <param name="text"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string FromKebabCase(this string text, CultureInfo culture = null)
        {
            return text.Replace("-", " ", true, culture ?? CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// kebab-case
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToKebabCase(this string text)
        {
            return text.ToNormalized(culture: CultureInfo.InvariantCulture).ToLowerInvariant().Replace(" ", "-", true, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Factory to Factories
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToCollected(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (text.EndsWith("ch", StringComparison.InvariantCultureIgnoreCase)
                || text.EndsWith("x", StringComparison.InvariantCultureIgnoreCase)
                || text.EndsWith("sh", StringComparison.InvariantCultureIgnoreCase)
                || text.EndsWith("s", StringComparison.InvariantCultureIgnoreCase))
            {
                text += "es";
            }
            else if (text.EndsWith("fe", StringComparison.InvariantCultureIgnoreCase)
                     || text.EndsWith("f", StringComparison.InvariantCultureIgnoreCase))
            {
                text += "s";
            }
            else if (text.EndsWith("y", StringComparison.InvariantCultureIgnoreCase))
            {
                if (text.Length <= 1)
                    return text;

                var prevChar = text[^2];
                if (prevChar == 'o' || prevChar == 'a' || prevChar == 'e' || prevChar == 'i')
                {
                    text += "s";
                }
                else
                {
                    text = text.Substring(0, text.Length - 1) + "ies";
                }
            }
            else if (text.EndsWith("o", StringComparison.InvariantCultureIgnoreCase))
            {
                if (text.Length <= 1)
                    return text;

                var prevChar = text[^2];
                if (prevChar == 'o' || prevChar == 'a' || prevChar == 'e' || prevChar == 'i')
                {
                    text += "s";
                }
                else
                {
                    text += "es";
                }
            }
            else
            {
                text += "s";
            }

            return text;
        }

        /// <summary>
        /// Gets only words and digits from string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ignoreSpace">Ignore space between words</param>
        /// <returns></returns>
        public static string ToUnescaped(this string text, bool ignoreSpace = false)
        {
            if (!string.IsNullOrEmpty(text))
                text = Regex.Replace(text, @"[^\w\d-. ]", "");

            if (ignoreSpace)
                text = text.Replace(" ", string.Empty);

            return text;
        }

        public static string ToNormalized(this string text, bool ignoreSpace = false, CultureInfo culture = null, bool forceToTitleCase = false, params string[] noNeedToNormalized)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var currentCulture = culture ?? CultureInfo.CurrentCulture;

            // The only way we can recognize if language supports upper-lower cases
            if (currentCulture.TextInfo.IsRightToLeft)
                return text;

            var textInfo = currentCulture.TextInfo;
            var result = new StringBuilder();
            var stringArray = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (stringArray.Length == 1)
            {
                if (ignoreSpace)
                    return text;

                // ThisIsFake
                // IELTS&TOMER

                var str = stringArray[0];
                if (str.Length == 1)
                    return str;

                if (str.Equals(str.ToLower(currentCulture)))
                    return textInfo.ToTitleCase(str);

                const string escapedChars = @"!@#$%^&*()_+?><';/.,\|`~=-";
                var startingIndex = 0;

                var canCarryOn = true;
                while (canCarryOn)
                {
                    var currentWord = string.Empty;
                    if (startingIndex > str.Length)
                    {
                        canCarryOn = false;
                    }
                    else
                    {
                        currentWord = str.Substring(startingIndex);
                        canCarryOn = !string.IsNullOrEmpty(currentWord);
                    }
                    if (!canCarryOn)
                        break;

                    var currentChar = currentWord.FirstOrDefault();
                    var currentCharIndex = str.IndexOf(currentChar, startingIndex);

                    if (char.IsUpper(currentChar) || char.IsLower(currentChar))
                    {
                        Func<char, bool> desiredCharacter;

                        if (currentCharIndex == str.Length - 1)
                        {
                            result.Append(currentChar);
                            startingIndex = str.Length;
                        }
                        else
                        {
                            var targetChar = str[currentCharIndex + 1];
                            if (!char.IsUpper(targetChar))
                            {
                                desiredCharacter = x =>
                                    char.IsUpper(x) || char.IsDigit(x) || escapedChars.Contains(x);
                            }
                            else
                            {
                                desiredCharacter = x =>
                                    char.IsLower(x) || char.IsDigit(x) || escapedChars.Contains(x);
                            }

                            var startOfNextWord = currentWord
                                .Skip(1)
                                .FirstOrDefault(desiredCharacter);
                            var startOfNextWordIndex = startOfNextWord != 0
                                ? string.Join("", str.Skip(1)).IndexOf(startOfNextWord) + 1
                                : str.Length;
                            var endCharIndex = startOfNextWordIndex - 1;

                            var word = str[new Range(currentCharIndex, endCharIndex + 1)];
                            result.Append(word);
                            result.Append(" ");

                            startingIndex = startOfNextWordIndex;
                        }
                    }
                    else if (char.IsDigit(currentChar) || escapedChars.Contains(currentChar))
                    {
                        if (currentCharIndex > 0)
                        {
                            var prevWord = str[new Range(0, currentCharIndex)];
                            result.Append(currentChar);
                        }
                        else
                        {
                            result.Append(currentChar);
                        }
                        result.Append(" ");
                        startingIndex = currentCharIndex + 1;
                    }
                }
            }
            else
            {
                // This Is Fake
                foreach (var str in stringArray)
                {
                    var tempLower = str.ToLower(currentCulture);

                    var finalString = str;
                    if (forceToTitleCase)
                    {
                        if (str.Length > 0)
                        {
                            var firstChar = str[0];
                            finalString = firstChar.ToString().ToUpper(culture);

                            var restOfChars = str[new Range(1, ^0)];
                            finalString += restOfChars.ToString().ToLower(culture);
                        }
                    }
                    else
                    {
                        if (str.Equals(tempLower, StringComparison.CurrentCulture))
                            finalString = textInfo.ToTitleCase(str);
                    }

                    result.Append(finalString);

                    if (!ignoreSpace)
                        result.Append(" ");
                }
            }

            return result.ToString().Trim();
        }

        public static string Replace(this string str, IEnumerable<string> oldValues, string newValue)
        {
            return oldValues?.Any() != true
              ? str
              : oldValues.Aggregate(str, (current, oldValue) => current.Replace(oldValue, newValue));
        }

        /// <summary>
        /// Convert persian non-unicode digits ( example 6 ) to english unicode digits
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string ConvertDigitsToLatin(this string s)
        {
            var sb = new StringBuilder();
            foreach (var t in s)
            {
                switch (t)
                {
                    //Persian digits
                    case '\u06f0':
                        sb.Append('0');
                        break;

                    case '\u06f1':
                        sb.Append('1');
                        break;

                    case '\u06f2':
                        sb.Append('2');
                        break;

                    case '\u06f3':
                        sb.Append('3');
                        break;

                    case '\u06f4':
                        sb.Append('4');
                        break;

                    case '\u06f5':
                        sb.Append('5');
                        break;

                    case '\u06f6':
                        sb.Append('6');
                        break;

                    case '\u06f7':
                        sb.Append('7');
                        break;

                    case '\u06f8':
                        sb.Append('8');
                        break;

                    case '\u06f9':
                        sb.Append('9');
                        break;

                    //Arabic digits
                    case '\u0660':
                        sb.Append('0');
                        break;

                    case '\u0661':
                        sb.Append('1');
                        break;

                    case '\u0662':
                        sb.Append('2');
                        break;

                    case '\u0663':
                        sb.Append('3');
                        break;

                    case '\u0664':
                        sb.Append('4');
                        break;

                    case '\u0665':
                        sb.Append('5');
                        break;

                    case '\u0666':
                        sb.Append('6');
                        break;

                    case '\u0667':
                        sb.Append('7');
                        break;

                    case '\u0668':
                        sb.Append('8');
                        break;

                    case '\u0669':
                        sb.Append('9');
                        break;

                    default:
                        sb.Append(t);
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Fixes common writing mistakes caused by using a bad keyboard layout,
        ///     such as replacing Arabic Ye with Persian one and so on ...
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        public static string ApplyPersianYeKe(this string text)
        {
            return string.IsNullOrEmpty(text)
                ? string.Empty
                : text.Replace(ArabicYeChar, PersianYeChar).Replace(ArabicKeChar, PersianKeChar).Trim();
        }

        private const char ArabicKeChar = (char)1603;
        private const char ArabicYeChar = (char)1610;
        private const char PersianKeChar = (char)1705;
        private const char PersianYeChar = (char)1740;

        /// <summary>
        /// Adds &zwnj; ( half-space) char between word and prefix/suffix
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        public static string ApplyHalfSpaceRule(this string text)
        {
            //put zwnj between word and prefix (mi* nemi*)
            var phase1 = Regex.Replace(text, @"\s+(ن?می)\s+", " $1‌");

            //put zwnj between word and suffix (*tar *tarin *ha *haye)
            return Regex.Replace(phase1, @"\s+(تر(ی(ن)?)?|ها(ی)?)\s+", "‌$1 ");
        }

        public static string FixPersian(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (!text.ContainsFarsi())
                return text;

            return text
                .ApplyModeratePersianRules()
                .ConvertDigitsToLatin()
                .Replace((char)1610, (char)1740)
                .Replace((char)1603, (char)1705)
                .Trim();
        }

        /// <summary>
        ///     Replaces more than one ! or ? mark with just one
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        public static string CleanupExtraMarks(this string text)
        {
            var phrase = Regex.Replace(text, "(!){2,}", "$1");
            return Regex.Replace(phrase, "(؟){2,}", "$1");
        }

        /// <summary>
        ///     Removes unnecessary zwnj char that are succeeded/preceded by a space
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        public static string CleanupZwnj(this string text)
        {
            return Regex.Replace(text, @"\s+‌|‌\s+", " ");
        }

        /// <summary>
        ///     Does text contain Persian characters?
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>true/false</returns>
        public static bool ContainsFarsi(this string text)
        {
            return Regex.IsMatch(text, @"[\u0600-\u06FF]");
        }

        /// <summary>
        ///     Converts ه ی to ه‌ی
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        public static string YeHeHalfSpace(this string text)
        {
            return Regex.Replace(text, @"(\S)(ه[\s‌]+[یي])(\s)", "$1ه‌ی‌$3"); // fix zwnj
        }
    }
}