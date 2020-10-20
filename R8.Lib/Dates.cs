using System;
using System.Globalization;

namespace R8.Lib
{
    public static class Dates
    {
        public static DateTime FromUnixTime(this long unixTimeStamp)
        {
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
            return dateTimeOffset.UtcDateTime;
        }

        public static long ToUnixTime(this DateTime dateTime)
        {
            var unixSeconds = new DateTimeOffset(dateTime).ToUnixTimeSeconds();
            return long.Parse(unixSeconds.ToString());
        }

        public static DateTime PersianToGregorian(this string persianDate)
        {
            try
            {
                var userInput = persianDate;
                var userDateParts = userInput.Split(new[]
                {
            "/", "-"
          }, StringSplitOptions.None);
                var userYear = int.Parse(userDateParts[0]);
                var userMonth = int.Parse(userDateParts[1]);
                var userDay = int.Parse(userDateParts[2]);

                var persianCalendar = new PersianCalendar();
                return persianCalendar.ToDateTime(userYear, userMonth, userDay, 0, 0, 0, 0);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static string GregorianToPersian(this DateTime dt, bool truncateTime, bool showSecs = false)
        {
            var pc = new PersianDateTime(dt)
            {
                ShowTime = !truncateTime,
                ShowTimeSecond = showSecs
            };

            var pdt = pc.ToString();
            return pdt;
        }
    }
}