using NodaTime;

using System;
using System.Globalization;
using System.Reflection;

namespace R8.Lib
{
    public static class Dates
    {
        public static DateTime FromUnixTime(this long unixTimeStamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).UtcDateTime;
        }

        public static long ToUnixTime(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Represents a Local Datetime by specific TimeZone
        /// </summary>
        /// <param name="utcDateTime">UTC DateTime to convert</param>
        /// <param name="timeZonePlaceName">Specific TimeZone ( such as "Asia/Tehran" )</param>
        /// <returns>Local DateTime for specific TimeZone</returns>
        public static DateTime GetLocalDateTime(this DateTime utcDateTime, string timeZonePlaceName = "Asia/Tehran")
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
                throw new AmbiguousMatchException($"'{nameof(utcDateTime)}' should be kind of {DateTimeKind.Utc}");

            var timeZone = DateTimeZoneProviders.Tzdb[timeZonePlaceName];
            return utcDateTime.GetLocalDateTime(timeZone);
        }

        /// <summary>
        /// Represents a Local Datetime by specific TimeZone
        /// </summary>
        /// <param name="utcDateTime">UTC DateTime to convert</param>
        /// <param name="timeZone">Specific TimeZone</param>
        /// <returns>Local DateTime for specific TimeZone</returns>
        public static DateTime GetLocalDateTime(this DateTime utcDateTime, DateTimeZone timeZone)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
                throw new AmbiguousMatchException($"'{nameof(utcDateTime)}' should be kind of {DateTimeKind.Utc}");

            var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc));
            var result = instant.InZone(timeZone).ToDateTimeUnspecified();
            return result;
        }

        public static DateTime PersianToGregorian(string persianDate)
        {
            var userInput = persianDate;
            var userDateParts = userInput.Split(new[] { "/", "-" }, StringSplitOptions.None);
            var userYear = int.Parse(userDateParts[0]);
            var userMonth = int.Parse(userDateParts[1]);
            var userDay = int.Parse(userDateParts[2]);

            var persianCalendar = new PersianCalendar();
            var dateTime = persianCalendar.ToDateTime(userYear, userMonth, userDay, 0, 0, 0, 0);
            return dateTime;
        }

        public static string ToPersianDateTime(this DateTime dt, bool truncateTime, bool showSecs = false)
        {
            var pc = new PersianDateTime(dt)
            {
                ShowTime = !truncateTime,
                ShowTimeSecond = showSecs
            };

            var pdt = pc.ToString();
            return pdt;
        }
    }
}