using System;
using System.Globalization;
using System.Text;

namespace R8.Lib
{
    public class PersianDateTime
    {
        public int Hour { get; }
        public int Minute { get; }
        public int Year { get; }
        public int Month { get; }
        public int Second { get; }
        public int DayOfMonth { get; }
        public int MonthDays { get; }
        public bool ShowTime { get; set; }
        public bool ShowTimeSecond { get; set; }

        public DayOfWeek DayOfWeek { get; }

        public string DayOfWeekPersian =>
            DayOfWeek switch
            {
                DayOfWeek.Friday => "جمعه",
                DayOfWeek.Monday => "دوشنبه",
                DayOfWeek.Sunday => "یکشنبه",
                DayOfWeek.Thursday => "پنجشنبه",
                DayOfWeek.Tuesday => "سه شنبه",
                DayOfWeek.Wednesday => "چهارشنبه",
                DayOfWeek.Saturday => "شنبه",
                _ => "شنبه"
            };

        public PersianDateTime(DateTime dateTime)
        {
            var persianCalendar = new PersianCalendar();

            Year = persianCalendar.GetYear(dateTime);
            Month = persianCalendar.GetMonth(dateTime);
            DayOfMonth = persianCalendar.GetDayOfMonth(dateTime);
            DayOfWeek = persianCalendar.GetDayOfWeek(dateTime);
            MonthDays = CountDayOfMonth(Month);

            Hour = dateTime.Hour;
            Minute = dateTime.Minute;
            Second = dateTime.Second;
        }

        private static int CountDayOfMonth(int month)
        {
            int dayOfMonth;
            if (month >= 1 && month <= 6)
            {
                dayOfMonth = 31;
            }
            else if (month >= 7 && month <= 11)
            {
                dayOfMonth = 30;
            }
            else if (month == 12)
            {
                dayOfMonth = 29;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"{nameof(month)} should be in range between 1 and 12");
            }

            return dayOfMonth;
        }

        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, DayOfMonth, Hour, Minute, Second, new PersianCalendar());
        }

        public string Humanize()
        {
            var now = DateTime.UtcNow;
            var dt = ToDateTime().ToUniversalTime();

            var nowSeconds = now.ToUnixTime();
            var dtSeconds = dt.ToUnixTime();

            var division = nowSeconds - dtSeconds;
            if (division < 0)
                throw new ArgumentOutOfRangeException($"Desired date should be lower than today");

            var timeSpan = TimeSpan.FromSeconds(division);
            if (timeSpan.TotalSeconds < TimeSpan.FromMinutes(1).TotalSeconds) // زیر یک دقیقه
                return $"{division} ثانیه پیش";

            if (timeSpan.TotalSeconds >= TimeSpan.FromMinutes(1).TotalSeconds &&
                timeSpan.TotalSeconds < TimeSpan.FromMinutes(10).TotalSeconds) // بین 1 تا 10 دقیقه
                return $"{(int)timeSpan.TotalMinutes} دقیقه پیش";

            if (timeSpan.TotalSeconds >= TimeSpan.FromMinutes(10).TotalSeconds && timeSpan.TotalSeconds < TimeSpan.FromMinutes(30).TotalSeconds)
                return $"{((int)timeSpan.TotalMinutes % 5 == 0 ? (int)timeSpan.TotalMinutes : Math.Round(timeSpan.TotalMinutes / 5, 0) * 5)} دقیقه پیش";

            if (timeSpan.TotalSeconds >= TimeSpan.FromMinutes(30).TotalSeconds && timeSpan.TotalSeconds < TimeSpan.FromMinutes(45).TotalSeconds)
                return $"نیم ساعت پیش";

            if (timeSpan.TotalSeconds >= TimeSpan.FromMinutes(45).TotalSeconds && timeSpan.TotalSeconds < TimeSpan.FromHours(1).TotalSeconds) // ده دقیقه به بالا
                return $"کمتر از یک ساعت پیش";

            if (timeSpan.TotalSeconds >= TimeSpan.FromHours(1).TotalSeconds && timeSpan.TotalSeconds < TimeSpan.FromDays(1).TotalSeconds)
                return $"{(int)timeSpan.TotalHours} ساعت پیش";

            if (timeSpan.TotalSeconds >= TimeSpan.FromDays(1).TotalSeconds && timeSpan.TotalSeconds < TimeSpan.FromDays(7).TotalSeconds)
                return $"{(int)timeSpan.TotalDays} روز پیش";

            if (timeSpan.TotalSeconds >= TimeSpan.FromDays(7).TotalSeconds && timeSpan.TotalSeconds < TimeSpan.FromDays(30).TotalSeconds)
                return $"{(int)timeSpan.TotalDays / 7} هفته پیش";

            if (timeSpan.TotalSeconds >= TimeSpan.FromDays(30).TotalSeconds && timeSpan.TotalSeconds < TimeSpan.FromDays(30 * 12).TotalSeconds)
                return $"{(int)timeSpan.TotalDays / 30} ماه پیش";

            return $"{(int)timeSpan.TotalDays / (12 * 30)} سال پیش";
        }

        public override string ToString()
        {
            var date = new StringBuilder();
            const string dateSeparator = "/";
            const string timeSeparator = ":";

            date.Append(Year.ToString("0000"))
                .Append(dateSeparator)
                .Append(Month.ToString("00"))
                .Append(dateSeparator)
                .Append(DayOfMonth.ToString("00"));

            var time = new StringBuilder();
            time.Append(Hour.ToString("00"))
                .Append(timeSeparator)
                .Append(Minute.ToString("00"));

            if (ShowTimeSecond)
            {
                time.Append(timeSeparator)
                    .Append(Second.ToString("00"));
            }

            return ShowTime ? $"{date} {time}" : $"{date}";
        }
    }
}