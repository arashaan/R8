using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace R8.Lib
{
    /// <summary>
    /// Represents an instance of <see cref="PersianDateTime"/>
    /// </summary>
    public struct PersianDateTime
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
        /// Gets the day of the week represented by this instance
        /// </summary>
        /// <returns>An enumerated constant that indicates the day of the week by this <see cref="PersianDateTime"/> value.</returns>
        public DayOfWeek DayOfWeek { get; }

        /// <summary>
        /// Represents an instance of <see cref="PersianDateTime"/> for <c>DateTime.Now</c>
        /// </summary>
        public static PersianDateTime Now => GetFromDateTime(DateTime.Now);

        private static bool TryParseTime(string time, out int? hour, out int? minute, out int? second)
        {
            hour = null;
            minute = null;
            second = null;

            var parts = time.Split(new[] { ":" }, StringSplitOptions.None).ToList();
            if (!parts.Any())
                return false;

            var list = new List<int>();
            foreach (var part in parts)
            {
                var parse = int.TryParse(part, out var num);
                if (!parse)
                    return false;

                list.Add(num);
            }

            if (list.Any(x => x < 0))
                return false;

            switch (parts.Count)
            {
                case 3:
                    hour = list[0];
                    minute = list[1];
                    second = list[2];
                    return true;

                case 2:
                    hour = list[0];
                    minute = list[1];
                    second = 0;
                    return true;

                default:
                    return false;
            }
        }

        private static bool TryParseDate(string date, out int? year, out int? month, out int? day)
        {
            year = null;
            month = null;
            day = null;

            var parts = date.Split(new[] { "/", "-" }, StringSplitOptions.None).ToList();
            if (parts.Count == 0)
                return false;

            var list = new List<int>();
            foreach (var part in parts)
            {
                var parse = int.TryParse(part, out var num);
                if (!parse)
                    return false;

                list.Add(num);
            }

            if (list.Any(x => x <= 0))
                return false;

            var yearIndex = 0;
            while (yearIndex <= 2)
            {
                var tempYear = list[yearIndex];
                if (tempYear >= 1300)
                {
                    year = tempYear;
                    break;
                }

                yearIndex++;
            }

            if (year == null)
                return false;

            list.RemoveAt(yearIndex);
            if (list.All(x => x <= 12))
            {
                month = list[0];
                day = list[1];
            }
            else
            {
                day = list.First(x => x > 12);
                month = list.First(x => x <= 12);
            }

            return true;
        }

        /// <summary>
        /// Converts the string representation of an <see cref="PersianDateTime"/> instance.
        /// </summary>
        /// <param name="s">An <see cref="string"/> containing Persian date time format</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A <see cref="PersianDateTime"/> instance</returns>
        public static PersianDateTime TryParse(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            var sectionSplit = new[] { " " };
            var splitText = s.Split(sectionSplit, StringSplitOptions.None).ToList();
            switch (splitText.Count)
            {
                case 1:
                    {
                        var date = splitText[0];
                        var tryParseDate = TryParseDate(date, out var year, out var month, out var day);
                        if (tryParseDate)
                            return new PersianDateTime(year.Value, month.Value, day.Value, 0, 0, 1);

                        var tryParseTime = TryParseTime(date, out var hour, out var minute, out var second);
                        if (tryParseTime)
                            return new PersianDateTime(Now.Year, Now.Month, Now.Day, hour.Value, minute.Value, second.Value);

                        throw new ArgumentException($"{date} is not in expected format.");
                    }

                case 2:
                default:
                    {
                        var date = splitText[0];
                        var tryParseDate = TryParseDate(date, out var year, out var month, out var day);
                        if (!tryParseDate)
                            throw new ArgumentException($"{date} is not in expected format for date.");

                        var time = splitText[1];
                        var tryParseTime = TryParseTime(time, out var hour, out var minute, out var second);
                        if (!tryParseTime)
                            throw new ArgumentException($"{date} is not in expected format for time.");

                        return new PersianDateTime(year.Value, month.Value, day.Value, hour.Value, minute.Value, second.Value);
                    }
            }
        }

        /// <summary>
        /// Initializes an instance of <see cref="PersianDateTime"/>
        /// </summary>
        /// <param name="dateTime">An initiated <see cref="DateTime"/></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static PersianDateTime GetFromDateTime(DateTime dateTime)
        {
            var persianCalendar = new PersianCalendar();

            var year = persianCalendar.GetYear(dateTime);
            var month = persianCalendar.GetMonth(dateTime);
            var day = persianCalendar.GetDayOfMonth(dateTime);

            var hour = dateTime.Hour;
            var minute = dateTime.Minute;
            var second = dateTime.Second;
            return new PersianDateTime(year, month, day, hour, minute, second);
        }

        /// <summary>
        /// Initializes an instance of <see cref="PersianDateTime"/>
        /// </summary>
        /// <param name="year">Specific persian year</param>
        /// <param name="month">Specific persian month</param>
        /// <param name="day">Specific persian day</param>
        public PersianDateTime(int year, int month, int day) : this(year, month, day, 0, 0, 1)
        {
        }

        /// <summary>
        /// Initializes an instance of <see cref="PersianDateTime"/>
        /// </summary>
        /// <param name="year">Specific year</param>
        /// <param name="month">Specific month</param>
        /// <param name="day">Specific day</param>
        /// <param name="hour">Specific hour</param>
        /// <param name="minute">Specific minute</param>
        /// <param name="second">Specific second</param>
        /// <param name="gregorianDate">Set if parameters entered as <see cref="DateTime"/> components</param>
        public PersianDateTime(int year, int month, int day, int hour, int minute, int second, bool gregorianDate)
        {
            var dt = new DateTime(year, month, day, hour, minute, second);
            if (gregorianDate)
            {
                var pDt = GetFromDateTime(dt);
                this.Year = pDt.Year;
                this.Month = pDt.Month;
                this.Day = pDt.Day;
                this.Hour = pDt.Hour;
                this.Minute = pDt.Minute;
                this.Second = pDt.Second;
                this.DayOfWeek = pDt.DayOfWeek;
            }
            else
            {
                this.Year = year;
                this.Month = month;
                this.Day = day;
                this.Hour = hour;
                this.Minute = minute;
                this.Second = second;
                this.DayOfWeek = dt.DayOfWeek;
            }
        }

        /// <summary>
        /// Initializes an instance of <see cref="PersianDateTime"/>
        /// </summary>
        /// <param name="year">Specific persian year</param>
        /// <param name="month">Specific persian month</param>
        /// <param name="day">Specific persian day</param>
        /// <param name="hour">Specific hour</param>
        /// <param name="minute">Specific minute</param>
        /// <param name="second">Specific second</param>
        public PersianDateTime(int year, int month, int day, int hour, int minute, int second) : this(year, month, day, hour, minute, second, false)
        {
        }

        /// <summary>
        /// Represents days of specific month
        /// </summary>
        /// <returns>Days count</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int GetMonthDaysCount() => GetMonthDaysCount(Month);

        /// <summary>
        /// Represents days of specific month
        /// </summary>
        /// <param name="month">Month number</param>
        /// <returns>Days count</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int GetMonthDaysCount(int month)
        {
            if (month >= 1 && month <= 6)
                return 31;

            if (month >= 7 && month <= 11)
                return 30;

            if (month == 12)
                return 29;

            throw new ArgumentOutOfRangeException($"{nameof(month)} should be in range between 1 and 12");
        }

        /// <summary>
        /// Converts current <see cref="PersianDateTime"/> instance to <see cref="DateTime"/>
        /// </summary>
        /// <returns>An instance of <see cref="DateTime"/></returns>
        public DateTime ToDateTime() => new DateTime(Year, Month, Day, Hour, Minute, Second, new PersianCalendar());

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
                throw new ArgumentOutOfRangeException("Desired date should be lower than today");

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
            return ToString(true, true);
        }

        /// <summary>
        /// Represents Persian month name
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns>An <see cref="string"/> value that representing persian month name</returns>
        public string GetMonthName() => GetMonthName(Month);

        /// <summary>
        /// Represents Persian month name
        /// </summary>
        /// <param name="month">Persian month number</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns>An <see cref="string"/> value that representing persian month name</returns>
        public static string GetMonthName(int month)
        {
            if (month <= 0 || month > 12)
                throw new ArgumentOutOfRangeException($"{month} must be a value between 1 and 12");

            return month switch
            {
                1 => "فروردین",
                2 => "اردیبهشت",
                3 => "خرداد",
                4 => "تیر",
                5 => "مرداد",
                6 => "شهریور",
                7 => "مهر",
                8 => "آبان",
                9 => "آذر",
                10 => "دی",
                11 => "بهمن",
                12 => "اسفند",
                _ => null
            };
        }

        /// <summary>
        /// Gets the day of the week based on <see cref="DayOfWeek"/>
        /// </summary>
        /// <returns>An translated string of enumerated constant ( such as : <c>جمعه</c> )</returns>
        public string GetDayOfWeek() => GetDayOfWeek(DayOfWeek);

        /// <summary>
        /// Gets the day of the week based on <see cref="DayOfWeek"/>
        /// </summary>
        /// <param name="dayOfWeek">A enumerator constant that representing day of week</param>
        /// <returns>An translated string of enumerated constant ( such as : <c>جمعه</c> )</returns>
        public static string GetDayOfWeek(DayOfWeek dayOfWeek) =>
            dayOfWeek switch
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
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <param name="showDayOfWeek">Show persian day of week.</param>
        /// <returns>The fully qualified type name.</returns>
        public string ToString(bool showDayOfWeek)
        {
            var date = $"{Day} {GetMonthName()} {Year}";

            return showDayOfWeek
                ? $"{GetDayOfWeek()}، {date}"
                : date;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <param name="showTime">Show time</param>
        /// <param name="showTimeSecond">time second component in time</param>
        /// <returns>The fully qualified type name.</returns>
        public string ToString(bool showTime, bool showTimeSecond)
        {
            var date = $"{Year:0000}/{Month:00}/{Day:00}";
            var time = $"{Hour:00}:{Minute:00}";

            if (showTimeSecond)
                time += $":{Second:00}";

            return showTime
                ? $"{date} {time}"
                : $"{date}";
        }
    }
}