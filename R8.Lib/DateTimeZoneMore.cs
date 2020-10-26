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

        public override string ToString()
        {
            return
                $"(UTC{Offset.ToString("m", CultureInfo.CurrentCulture)}) {SystemTimeZone.DisplayName.Split(") ")[1]}";
        }
    }
}