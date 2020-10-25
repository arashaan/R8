using System;

using Xunit;

namespace R8.Lib.Test
{
    public class PersianDateTimeTests
    {
        [Fact]
        public void CallPersianDateTime_49MinutesAgo()
        {
            // Assets
            var dateTime = DateTime.Now.AddMinutes(-49);
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var relativeDateTime = persianDateTime.Humanize();

            // Arranges
            Assert.Equal("کمتر از یک ساعت پیش", relativeDateTime);
        }

        [Fact]
        public void CallPersianDateTime_13MonthsAgo()
        {
            // Assets
            var dateTime = DateTime.Now.AddDays(-30 * 13);
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var relativeDateTime = persianDateTime.Humanize();

            // Arranges
            Assert.Equal("1 سال پیش", relativeDateTime);
        }

        [Fact]
        public void CallPersianDateTime_40DaysAgo()
        {
            // Assets
            var dateTime = DateTime.Now.AddDays(-40);
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var relativeDateTime = persianDateTime.Humanize();

            // Arranges
            Assert.Equal("1 ماه پیش", relativeDateTime);
        }

        [Fact]
        public void CallPersianDateTime_13DaysAgo()
        {
            // Assets
            var dateTime = DateTime.Now.AddDays(-13);
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var relativeDateTime = persianDateTime.Humanize();

            // Arranges
            Assert.Equal("1 هفته پیش", relativeDateTime);
        }

        [Fact]
        public void CallToString_FullWithTimeWithSecond()
        {
            // Assets
            var dateTime = new DateTime(2020, 10, 25, 22, 1, 0);

            // Acts
            var pdate = new PersianDateTime(dateTime)
            {
                ShowTimeSecond = true,
                ShowTime = true
            };

            Assert.Equal("1399/08/04 22:01:00", pdate.ToString());
        }

        [Fact]
        public void CallToString_FullWithTime()
        {
            // Assets
            var dateTime = new DateTime(2020, 10, 25, 22, 1, 0);

            // Acts
            var pdate = new PersianDateTime(dateTime)
            {
                ShowTimeSecond = false,
                ShowTime = true
            };

            Assert.Equal("1399/08/04 22:01", pdate.ToString());
        }

        [Fact]
        public void CallToString_Full()
        {
            // Assets
            var dateTime = new DateTime(2020, 10, 25, 22, 1, 0);

            // Acts
            var pdate = new PersianDateTime(dateTime)
            {
                ShowTimeSecond = false,
                ShowTime = false
            };

            Assert.Equal("1399/08/04", pdate.ToString());
        }

        [Fact]
        public void CallPersianDateTime_ToDateTime()
        {
            // Assets
            var dateTime = DateTime.Now.Date;
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var relativeDateTime = persianDateTime.ToDateTime().Date;

            // Arranges
            Assert.Equal(dateTime, relativeDateTime);
        }

        [Fact]
        public void CallPersianDateTime_Tomorrow()
        {
            // Assets
            var dateTime = DateTime.Now.AddDays(1);
            var persianDateTime = new PersianDateTime(dateTime);

            // Arranges
            Assert.Throws<ArgumentOutOfRangeException>(() => persianDateTime.Humanize());
        }

        [Fact]
        public void CallPersianDateTime_DayOfWeek()
        {
            // Assets
            var dateTime = new DateTime(2020, 10, 25, 22, 1, 0);
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var day = persianDateTime.DayOfWeekPersian;

            // Arranges
            Assert.Equal("یکشنبه", day);
        }

        [Fact]
        public void CallPersianDateTime_3DaysAgo()
        {
            // Assets
            var dateTime = DateTime.Now.AddDays(-3);
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var relativeDateTime = persianDateTime.Humanize();

            // Arranges
            Assert.Equal("3 روز پیش", relativeDateTime);
        }

        [Fact]
        public void CallPersianDateTime_33MinutesAgo()
        {
            // Assets
            var dateTime = DateTime.Now.AddMinutes(-33);
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var relativeDateTime = persianDateTime.Humanize();

            // Arranges
            Assert.Equal("نیم ساعت پیش", relativeDateTime);
        }

        [Fact]
        public void CallPersianDateTime_17MinutesAgo()
        {
            // Assets
            var dateTime = DateTime.Now.AddMinutes(-17);
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var relativeDateTime = persianDateTime.Humanize();

            // Arranges
            Assert.Equal("15 دقیقه پیش", relativeDateTime);
        }

        [Fact]
        public void CallPersianDateTime_5MinutesAgo()
        {
            // Assets
            var dateTime = DateTime.Now.AddMinutes(-5);
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var relativeDateTime = persianDateTime.Humanize();

            // Arranges
            Assert.Equal("5 دقیقه پیش", relativeDateTime);
        }

        [Fact]
        public void CallPersianDateTime_10MinutesAgo()
        {
            // Assets
            var dateTime = DateTime.Now.AddMinutes(-10);
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var relativeDateTime = persianDateTime.Humanize();

            // Arranges
            Assert.Equal("10 دقیقه پیش", relativeDateTime);
        }

        [Fact]
        public void CallPersianDateTime_10SecondsAgo()
        {
            // Assets
            var dateTime = DateTime.Now.AddSeconds(-10);
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var relativeDateTime = persianDateTime.Humanize();

            // Arranges
            Assert.Equal("10 ثانیه پیش", relativeDateTime);
        }

        [Fact]
        public void CallPersianDateTime_OneHourAgo()
        {
            // Assets
            var dateTime = DateTime.Now.AddHours(-1);
            var persianDateTime = new PersianDateTime(dateTime);

            // Acts
            var relativeDateTime = persianDateTime.Humanize();

            // Arranges
            Assert.Equal("1 ساعت پیش", relativeDateTime);
        }
    }
}