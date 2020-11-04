using System;
using System.Globalization;
using System.Text;

namespace R8.Lib
{
    /// <summary>
    /// Represents an instance of <see cref="PersianDateTime"/>
    /// </summary>
    public class PersianDateTime
    {
        /// <summary>
        /// Gets the hour component of the date by this instance
        /// </summary>
        /// <returns>The hour component, expressed as a value between 0 and 23</returns>
        public int Hour { get; }

        /// <summary>
        /// Gets the minute component of the date by this instance
        /// </summary>
        /// <returns>The minute component, expressed as a value between 0 and 59</returns>
        public int Minute { get; }

        /// <summary>
        /// Gets the year component of the date by this instance
        /// </summary>
        /// <returns>The year between 1 and 9999</returns>
        public int Year { get; }

        /// <summary>
        /// Gets the month component of the date by this instance
        /// </summary>
        /// <returns>The month component, expressed as a value between 1 and 12</returns>
        public int Month { get; }

        /// <summary>
        /// Gets the second component of the date by this instance
        /// </summary>
        /// <returns>The second component, expressed as a value between 0 and 59</returns>
        public int Second { get; }

        /// <summary>
        /// Gets the day component of the date by this instance
        /// </summary>
        /// <returns>The day component, expressed as a value between 1 and 31 </returns>
        public int Day { get; }

        /// <summary>
        /// Gets count of the days in <see cref="Month"/>
        /// </summary>
        public int MonthDays { get; }

        /// <summary>
        /// Gets of sets boolean to let show Time in humanized string.
        /// </summary>
        public bool ShowTime { get; set; }

        /// <summary>
        /// Gets of sets boolean to let show Seconds in humanized string.
        /// </summary>
        public bool ShowTimeSecond { get; set; }

        /// <summary>
        /// Gets the day of the week represented by this instance
        /// </summary>
        /// <returns>An enumerated constant that indicates the day of the week by this <see cref="PersianDateTime"/> value.</returns>
        public DayOfWeek DayOfWeek { get; }

        /// <summary>
        /// Gets the day of the week based on <see cref="DayOfWeek"/>
        /// </summary>
        /// <returns>An translated string of enumerated constant ( such as : <c>جمعه</c> )</returns>
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

        /// <summary>
        /// Represents an instance of <see cref="PersianDateTime"/> for <c>DateTime.Now</c>
        /// </summary>
        public static PersianDateTime Now() => new PersianDateTime(DateTime.Now);

        /// <summary>
        /// Represents an instance of <see cref="PersianDateTime"/>
        /// </summary>
        /// <param name="dateTime">An initiated <see cref="DateTime"/></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PersianDateTime(DateTime dateTime)
        {
            var persianCalendar = new PersianCalendar();

            Year = persianCalendar.GetYear(dateTime);
            Month = persianCalendar.GetMonth(dateTime);
            Day = persianCalendar.GetDayOfMonth(dateTime);
            DayOfWeek = persianCalendar.GetDayOfWeek(dateTime);
            MonthDays = CountDayOfMonth(Month);

            Hour = dateTime.Hour;
            Minute = dateTime.Minute;
            Second = dateTime.Second;
        }

        /// <summary>
        /// Represents an instance of <see cref="PersianDateTime"/>
        /// </summary>
        /// <param name="year">Specific persian year</param>
        /// <param name="month">Specific persian month</param>
        /// <param name="day">Specific persian day</param>
        public PersianDateTime(int year, int month, int day) : this(year, month, day, 0, 0, 1)
        {
        }

        /// <summary>
        /// Represents an instance of <see cref="PersianDateTime"/>
        /// </summary>
        /// <param name="year">Specific persian year</param>
        /// <param name="month">Specific persian month</param>
        /// <param name="day">Specific persian day</param>
        /// <param name="hour">Specific hour</param>
        /// <param name="minute">Specific minute</param>
        /// <param name="second">Specific second</param>
        public PersianDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
        }

        /// <summary>
        /// Represents days of specific month
        /// </summary>
        /// <param name="month">Month number</param>
        /// <returns>Days count</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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

        /// <summary>
        /// Converts current <see cref="PersianDateTime"/> instance to <see cref="DateTime"/>
        /// </summary>
        /// <returns>An instance of <see cref="DateTime"/></returns>
        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day, Hour, Minute, Second, new PersianCalendar());
        }

        /// <summary>
        /// Represents Humanized text for this Persian DateTime
        /// </summary>
        /// <returns>For example : 10 ثانیه پیش</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
                return "نیم ساعت پیش";

            if (timeSpan.TotalSeconds >= TimeSpan.FromMinutes(45).TotalSeconds && timeSpan.TotalSeconds < TimeSpan.FromHours(1).TotalSeconds) // ده دقیقه به بالا
                return "کمتر از یک ساعت پیش";

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
                .Append(Day.ToString("00"));

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