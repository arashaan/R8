using NodaTime;
using NodaTime.TimeZones;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace R8.Lib
{
    public static class Dates
    {
        /// <summary>
        /// Returns a <see cref="DateTime"/> object according to given unix timestamp.
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns>A <see cref="DateTime"/> in Universal DateTime.</returns>
        public static DateTime FromUnixTime(long unixTimeStamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).UtcDateTime;
        }

        /// <summary>
        /// Returns a unix timestamp from given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>A <see cref="long"/> that indicates as Unix Timestamp.</returns>
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
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>Local DateTime for specific TimeZone</returns>
        public static DateTime GetLocalDateTime(this DateTime utcDateTime, DateTimeZone timeZone)
        {
            if (timeZone == null) 
                throw new ArgumentNullException(nameof(timeZone));
            
            if (utcDateTime.Kind != DateTimeKind.Utc)
                throw new AmbiguousMatchException($"'{nameof(utcDateTime)}' should be kind of {DateTimeKind.Utc}");

            var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc));
            var result = instant.InZone(timeZone).ToDateTimeUnspecified();
            return result;
        }

        /// <summary>
        /// Returns an <see cref="object"/> that representing information about given timezone.
        /// </summary>
        /// <param name="timeZoneId">An <see cref="string"/> that representing timezone's id.</param>
        /// <param name="isSystemTimeZone">Checks whether given parameter is known id for System's native timezones or Noda timezones.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="TimeZoneNotFoundException"></exception>
        /// <returns>An <see cref="DateTimeZoneMore"/> object.</returns>
        /// <remarks>parameter should be like <c>Iran Standard Time</c></remarks>
        public static DateTimeZoneMore GetNodaTimeZone(string timeZoneId, bool isSystemTimeZone = true)
        {
            if (timeZoneId == null)
                throw new ArgumentNullException(nameof(timeZoneId));

            var source = TzdbDateTimeZoneSource.Default;
            var tzdbWindows = isSystemTimeZone
                ? source.TzdbToWindowsIds.FirstOrDefault(x => x.Value.Equals(timeZoneId))
                : source.TzdbToWindowsIds.FirstOrDefault(x => x.Key.Equals(timeZoneId));
            if (string.IsNullOrEmpty(tzdbWindows.Value))
                throw new TimeZoneNotFoundException(nameof(timeZoneId));
            
            TimeZoneInfo timeZone;
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(tzdbWindows.Value);
            }
            catch (Exception e)
            {
                timeZone = null;
            }
            
            var zone = DateTimeZoneProviders.Tzdb[tzdbWindows.Key];
            var offset = zone.GetUtcOffset(SystemClock.Instance.GetCurrentInstant());
            return new DateTimeZoneMore
            {
                NodaTimeZone = zone,
                Offset = offset,
                SystemTimeZone = timeZone
            };
        }

        /// <summary>
        /// Returns a collection of <see cref="DateTimeZoneMore"/> objects.
        /// </summary>
        /// <returns>An <see cref="List{T}"/> object that contains timezone objects.</returns>
        public static List<DateTimeZoneMore> GetNodaTimeZones()
        {
            var source = TzdbDateTimeZoneSource.Default;
            var timeZoneIds = source.TzdbToWindowsIds;

            var result = new List<DateTimeZoneMore>();
            foreach (var (nodaId, systemId) in timeZoneIds)
            {
                try
                {
                    var zone = DateTimeZoneProviders.Tzdb[nodaId];
                    var offset = zone.GetUtcOffset(SystemClock.Instance.GetCurrentInstant());
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(systemId);

                    var duplicate = result.Any(x => x.SystemTimeZone.Equals(timeZone));
                    if (duplicate)
                        continue;

                    var more = new DateTimeZoneMore
                    {
                        NodaTimeZone = zone,
                        Offset = offset,
                        SystemTimeZone = timeZone
                    };
                    result.Add(more);
                }
                catch (Exception e)
                {
                }
            }

            return Enumerable.OrderBy(result, x => x.Offset).ToList();
        }
    }
}