﻿using NodaTime;
using NodaTime.TimeZones;

using System;
using System.Collections.Generic;
using System.Linq;
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

        public static DateTimeZoneMore GetNodaTimeZone(string name)
        {
            var source = TzdbDateTimeZoneSource.Default;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(name);

            var id = source.TzdbToWindowsIds.FirstOrDefault(x => x.Value.Equals(timeZone.Id)).Key;

            var zone = DateTimeZoneProviders.Tzdb[id];
            var offset = zone.GetUtcOffset(SystemClock.Instance.GetCurrentInstant());
            return new DateTimeZoneMore
            {
                NodaTimeZone = zone,
                Offset = offset,
                SystemTimeZone = timeZone
            };
        }

        public static IEnumerable<DateTimeZoneMore> GetNodaTimeZones()
        {
            var systemTimeZones = TimeZoneInfo.GetSystemTimeZones();
            return from timeZone in systemTimeZones
                   let more = GetNodaTimeZone(timeZone.Id)
                   orderby more.Offset
                   select more;
        }

        public static string ToPersianDateTime(this DateTime dt, bool truncateTime, bool showSecs = false)
        {
            return PersianDateTime.GetFromDateTime(dt).ToString(!truncateTime, showSecs);
        }
    }
}