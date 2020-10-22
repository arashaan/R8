using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace R8.Lib
{
    public static class TimeZoneExtensions
    {
        public static DateTime GetUserDateTime(this DateTimeZone timeZone, DateTime? utcTime = null)
        {
            utcTime ??= DateTime.UtcNow;
            if (((DateTime)utcTime).Kind != DateTimeKind.Utc)
                throw new AmbiguousMatchException($"'{nameof(utcTime)}' should be kind of {DateTimeKind.Utc}");

            return utcTime.Value.ToTimeZone(timeZone);
        }

        public static DateTime ToTimeZone(this DateTime utcDateTime, DateTimeZone timeZone)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
                throw new AmbiguousMatchException($"'{nameof(utcDateTime)}' should be kind of {DateTimeKind.Utc}");

            var desiredTimeZone = timeZone.GetTimeZone();
            var dt = utcDateTime.ToTimeZoneTime(desiredTimeZone);
            return dt;
        }

        public static DateTimeZone GetDateTimeZone(this TimeZoneInfo timeZone)
        {
            return new DateTimeZone(timeZone);
        }

        public static IEnumerable<DateTimeZone> GetFinalTimeZones(this IReadOnlyCollection<TimeZoneInfo> timeZones)
        {
            if (timeZones == null) throw new ArgumentNullException(nameof(timeZones));

            var result = timeZones
                .Select(tz => new DateTimeZone(tz))
                .ToList();
            return result;
        }

        /// <summary>
        /// Returns TimeZone adjusted time for a given from a Utc or local time.
        /// Date is first converted to UTC then adjusted.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static DateTime ToTimeZoneTime(this DateTime time, string timeZoneId = "Pacific Standard Time")
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return time.ToTimeZoneTime(tzi);
        }

        /// <summary>
        /// Returns TimeZone adjusted time for a given from a Utc or local time.
        /// Date is first converted to UTC then adjusted.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static DateTime ToTimeZoneTime(this DateTime time, TimeZoneInfo tzi)
        {
            var dt = TimeZoneInfo.ConvertTimeFromUtc(time, tzi);
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
            return dt;
        }
    }

    //public sealed class TimeZoneService
    //{
    //    private readonly IJSRuntime _jsRuntime;

    //    private TimeSpan? _userOffset;

    //    public TimeZoneService(IJSRuntime jsRuntime)
    //    {
    //        _jsRuntime = jsRuntime;
    //    }

    //    public async ValueTask<DateTimeOffset> GetLocalDateTime(DateTimeOffset dateTime)
    //    {
    //        if (_userOffset == null)
    //        {
    //            var offsetInMinutes = await _jsRuntime.InvokeAsync<int>("ecoTimeZone");
    //            _userOffset = TimeSpan.FromMinutes(-offsetInMinutes);
    //        }

    //        return dateTime.ToOffset(_userOffset.Value);
    //    }
    //}
}