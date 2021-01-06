using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace R8.Lib
{
    public static class RegexPatterns
    {
        /// <summary>
        /// Only lowercase english letters and digits, plus DASH
        /// </summary>
        public const string Key = @"^([a-z0-9-]+)$";

        /// <summary>
        /// Only persian and english digits in following pattern: +9XXXXXXXXX
        /// </summary>
        public const string Mobile = @"\+([0-9]|[۰-۹]){9,}";

        /// <summary>
        /// Only english digits
        /// </summary>
        public const string NumbersOnly = @"^[\d]*$";

        /// <summary>
        /// Only english letters, plus SPACE
        /// </summary>
        public const string EnglishText = @"^[\w ]+$";

        /// <summary>
        /// Only english digits in PHONE number preceding area code in following pattern: XXX-XXXXXXXX
        /// </summary>
        public const string Phone = "[0-9]{1,}-[1-8]([0-9]){7}";

        /// <summary>
        /// Only english letters and digits, plus AT-SIGN, DOT, UNDERSCORE and DASH
        /// </summary>
        public const string SocialAccount = @"[\d\w@._-]+";

        /// <summary>
        /// Only english digits in time pattern: XX:XX
        /// </summary>
        public const string Time = "^([01]?[0-9]|2[0-3]):[0-5][0-9]?";

        /// <summary>
        /// Only english digits in iranian national id pattern : XXXYYYYYYY
        /// </summary>
        public const string IranNationalCode = @"([\d]{3})([\d]{7})";

        /// <summary>
        /// Only english letters and digits in email pattern: XXXXX@YYYY.ZZZ
        /// </summary>
        public const string EmailAddress = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$";

        /// <summary>
        /// Only english digits in iranian calendar pattern: YYYY/MM/DD
        /// </summary>
        public const string IranDate =
            @"^((1[34]\d{2})\/((0?[1-6]\/(0?[1-9]|[12]\d|3[01]))|(0?[7-9]\/(0?[1-9]|[12]\d|30))|(1[01]\/(0?[1-9]|[12]\d|30))|(12\/(0?[1-9]|[12]\d))))$";
    }
}