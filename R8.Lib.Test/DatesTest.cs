using NodaTime;

using System;
using System.Reflection;

using Xunit;

namespace R8.Lib.Test
{
    public class DatesTest
    {
        [Fact]
        public void CallGetLocalDateTime_NotUtc()
        {
            // Assets
            var utcDateTime = new DateTime(2020, 10, 25, 19, 25, 0);

            // Arranges
            Assert.Throws<AmbiguousMatchException>(() => utcDateTime.GetLocalDateTime("Asia/Tehran"));
        }

        [Fact]
        public void CallGetLocalDateTime_NotUtc2()
        {
            // Assets
            var utcDateTime = new DateTime(2020, 10, 25, 19, 25, 0);

            // Arranges
            Assert.Throws<AmbiguousMatchException>(() => utcDateTime.GetLocalDateTime(DateTimeZoneProviders.Tzdb["Asia/Tehran"]));
        }

        [Fact]
        public void CallGetLocalDateTime_Tehran2()
        {
            // Assets
            var utcDateTime = DateTime.SpecifyKind(new DateTime(2020, 10, 25, 19, 25, 0), DateTimeKind.Utc);
            var tehranDateTime = new DateTime(2020, 10, 25, 22, 55, 0);
            var timeZone = DateTimeZoneProviders.Tzdb["Asia/Tehran"];

            // Acts
            var zoneDateTime = utcDateTime.GetLocalDateTime(timeZone);

            // Arranges
            Assert.Equal(tehranDateTime, zoneDateTime);
        }

        [Fact]
        public void CallGetLocalDateTime_Tehran()
        {
            // Assets
            var utcDateTime = DateTime.SpecifyKind(new DateTime(2020, 10, 25, 19, 25, 0), DateTimeKind.Utc);
            var tehranDateTime = new DateTime(2020, 10, 25, 22, 55, 0);

            // Acts
            var zoneDateTime = utcDateTime.GetLocalDateTime("Asia/Tehran");

            // Arranges
            Assert.Equal(tehranDateTime, zoneDateTime);
        }

        [Fact]
        public void CallGetLocalDateTime_Istanbul()
        {
            // Assets
            var utcDateTime = DateTime.SpecifyKind(new DateTime(2020, 10, 25, 19, 25, 0), DateTimeKind.Utc);
            var tehranDateTime = new DateTime(2020, 10, 25, 22, 25, 0);

            // Acts
            var zoneDateTime = utcDateTime.GetLocalDateTime("Europe/Istanbul");

            // Arranges
            Assert.Equal(tehranDateTime, zoneDateTime);
        }

        [Fact]
        public void CallPersianToGregorian()
        {
            // Assets
            var persianDate = "1399/08/04";
            var gregorianDate = new DateTime(2020, 10, 25, 0, 0, 0);

            // Acts
            var persian = Dates.PersianToGregorian(persianDate);

            // Arranges
            Assert.Equal(gregorianDate, persian);
        }

        [Fact]
        public void CallGregorianToPersian()
        {
            // Assets
            var gregorianDate = new DateTime(2020, 10, 25, 0, 0, 0);
            var persianDate = "1399/08/04";

            // Acts
            var persian = gregorianDate.ToPersianDateTime(true);

            // Arranges
            Assert.Equal(persianDate, persian);
        }
    }
}