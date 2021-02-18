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
        /// From e.g. 2021-01-30T13:02:11 to 2021-01-30
        /// </summary>
        public static string GetDateOnly(string datetime)
        {
            if (string.IsNullOrEmpty(datetime)) return "";

            return datetime.Substring(0, datetime.IndexOf('T'));
        }

        public static string GetDateOnly(this DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy");
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
    }
}