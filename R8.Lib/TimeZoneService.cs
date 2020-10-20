using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace R8.Lib
{
    public struct DateTimeZone
    {
        private readonly TimeSpan _offset;
        private readonly TimeZoneInfo _timeZone;

        public override string ToString()
        {
            return DisplayName;
        }

        public DateTimeZone(TimeZoneInfo timeZone)
        {
            _timeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));
            Id = timeZone.Id;

            var adjustmentRule = timeZone.GetAdjustmentRules()
                .FirstOrDefault(x => x.DateStart <= DateTime.UtcNow && x.DateEnd >= DateTime.UtcNow);
            _offset = adjustmentRule?.DaylightDelta != null
                ? timeZone.BaseUtcOffset + adjustmentRule.DaylightDelta
                : timeZone.BaseUtcOffset;
            TimeZoneCity = timeZone.DisplayName.Substring(timeZone.DisplayName.IndexOf(") ") + 2);
            Adjustment = $"{(_offset.Hours > 0 ? "+" : "-")}{_offset:hh\\:mm}";
            DisplayName = $"(GMT{Adjustment}) {TimeZoneCity}";
        }

        public static IEnumerable<DateTimeZone> GetOffsets(TimeSpan timeSpan)
        {
            var accidentTimeZones = GetTimeZones()
                .Where(x => x.GetOffset() == timeSpan)
                .ToArray();
            return accidentTimeZones;
        }

        public static DateTimeZone? FirstOrDefault(string offset, string id)
        {
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(offset))
                return null;

            if (!offset.StartsWith("+") && !offset.StartsWith("-"))
                return null;

            DateTimeZone? timeZone;
            try
            {
                timeZone = FindById(id);
            }
            catch
            {
                timeZone = null;
            }

            DateTimeZone finalTimeZone;
            if (timeZone == null)
            {
                var offsetSymbol = offset.Substring(0, 1);
                string offsetHourStr;
                var offsetMinuteStr = "0";
                if (offset.Contains("."))
                {
                    var offsetArr = offset.Substring(1).Split(".");
                    offsetHourStr = offsetArr[0];
                    offsetMinuteStr = offsetArr[1];
                    offsetMinuteStr = offsetMinuteStr == "5" ? "30" : "0";
                }
                else
                {
                    offsetHourStr = offset.Substring(1);
                }

                var hasValidHour = int.TryParse(offsetHourStr, out var offsetHour);
                var hasValidMinute = int.TryParse(offsetMinuteStr, out var offsetMinute);
                if (!hasValidHour || !hasValidMinute)
                    return null;

                offsetHour = offsetSymbol == "-" ? offsetHour * -1 : offsetHour;
                offsetMinute = offsetSymbol == "-" ? offsetMinute * -1 : offsetMinute;
                var offsetTs = new TimeSpan(0, offsetHour, offsetMinute, 0);
                var accidentTimeZones = GetOffsets(offsetTs).ToList();
                var accidentTimeZone = accidentTimeZones?.FirstOrDefault();
                if (accidentTimeZone?.HasTimeZone != true)
                    return null;

                finalTimeZone = (DateTimeZone)accidentTimeZone;
            }
            else
            {
                finalTimeZone = (DateTimeZone)timeZone;
            }

            return finalTimeZone;
        }

        public static DateTimeZone GetOffset(TimeSpan timeSpan)
        {
            var accidentTimeZones = GetTimeZones()
                .FirstOrDefault(x => x.GetOffset() == timeSpan);
            return accidentTimeZones;
        }

        public static IEnumerable<DateTimeZone> GetTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().GetFinalTimeZones();
        }

        public bool HasTimeZone => !string.IsNullOrEmpty(DisplayName);

        public string TimeZoneCity { get; set; }
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public string Adjustment { get; set; }

        public TimeSpan GetOffset()
        {
            return _offset;
        }

        public TimeZoneInfo GetTimeZone()
        {
            return _timeZone;
        }

        public static DateTimeZone FindById(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (id.Contains("Daylight", StringComparison.InvariantCultureIgnoreCase))
                id = id.Replace("Daylight", "Standard", StringComparison.InvariantCultureIgnoreCase);

            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(id);
                return new DateTimeZone(timeZone);
            }
            catch (InvalidTimeZoneException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //public TimeSpan GetOffset(DateTime? dt = null)
        //{
        //    var dt2 = dt ?? DateTime.UtcNow;
        //    return _timeZone.GetUtcOffset(dt2);
        //}
    }

    public static class TimeZoneExtensions
    {
        //public static string HumanizeByTimeZone(this DateTime utcDateTime, DateTimeZone timeZone, bool utcDate = false)
        //{
        //    return utcDateTime.ToTimeZone(timeZone)
        //        .Humanize(utcDate, timeZone.GetUserDateTime());
        //}

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