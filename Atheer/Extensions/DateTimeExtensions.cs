using System;
using System.Globalization;

namespace Atheer.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Sortable dy database, e.g. 2021-01-30T13:02:11
        /// </summary>
        public static string GetString(this DateTime dateTime)
        {
            return dateTime.ToString("s");
        }

        /// <summary>
        /// ISO 8601
        /// </summary>
        public static string GetDateOnly(string datetime)
        {
            if (string.IsNullOrEmpty(datetime)) return "";

            return datetime.Substring(0, datetime.IndexOf('T'));
        }

        /// <summary>
        /// ISO 8601
        /// </summary>
        public static string GetDateOnly(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Datetime must like e.g. 2021-01-30T13:02:11
        /// </summary>
        public static string GetLocalDateOnly(string date)
        {
            if (string.IsNullOrEmpty(date)) return "";
            
            var dateCreated = DateTime.Parse(date);
            return dateCreated.ToLocalTime().GetDateOnly();
        }

        public static DateTime FirstTickOfDay(this DateTime dt)
        {
            return dt.Date;
        }

        public static DateTime LastTickOfDay(this DateTime dt)
        {
            return dt.Date.AddDays(1).AddTicks(-1);
        }

        public static int MillisecondsUntilNextDay(this DateTime dt)
        {
            var now = DateTime.UtcNow;
            return (int) (now.LastTickOfDay() - now).TotalMilliseconds;
        }
    }
}