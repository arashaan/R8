using NodaTime;

using System;
using System.Collections.Generic;
using System.Globalization;

namespace R8.Lib
{
    public class DateTimeZoneMore : IEquatable<DateTimeZoneMore>, IEqualityComparer<DateTimeZoneMore>
    {
        public TimeZoneInfo SystemTimeZone { get; set; }
        public DateTimeZone NodaTimeZone { get; set; }
        public Offset Offset { get; set; }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DateTimeZoneMore)obj);
        }

        public override string ToString()
        {
            return
                $"(UTC{Offset.ToString("m", CultureInfo.CurrentCulture)}) {SystemTimeZone.DisplayName.Split(") ")[1]}";
        }

        public bool Equals(DateTimeZoneMore other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(SystemTimeZone, other.SystemTimeZone) && Equals(NodaTimeZone, other.NodaTimeZone) && Offset.Equals(other.Offset);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SystemTimeZone, NodaTimeZone, Offset);
        }

        public bool Equals(DateTimeZoneMore x, DateTimeZoneMore y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return Equals(x.SystemTimeZone, y.SystemTimeZone) && Equals(x.NodaTimeZone, y.NodaTimeZone) && x.Offset.Equals(y.Offset);
        }

        public int GetHashCode(DateTimeZoneMore obj)
        {
            return HashCode.Combine(obj.SystemTimeZone, obj.NodaTimeZone, obj.Offset);
        }
    }
}