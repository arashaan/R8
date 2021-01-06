using NodaTime;

using System;
using System.Globalization;

namespace R8.Lib
{
    public class DateTimeZoneMore
    {
        public TimeZoneInfo SystemTimeZone { get; set; }
        public DateTimeZone NodaTimeZone { get; set; }
        public Offset Offset { get; set; }

        public override bool Equals(object? obj)
        {
            if (!(obj is DateTimeZoneMore dtMore))
                return false;

            var errors = 0;
            if (!dtMore.NodaTimeZone.Equals(NodaTimeZone))
                errors++;

            if (!dtMore.Offset.Equals(Offset))
                errors++;

            if (!dtMore.SystemTimeZone.Equals(SystemTimeZone))
                errors++;

            return errors == 0;
        }

        public override string ToString()
        {
            return
                $"(UTC{Offset.ToString("m", CultureInfo.CurrentCulture)}) {SystemTimeZone.DisplayName.Split(") ")[1]}";
        }
    }
}