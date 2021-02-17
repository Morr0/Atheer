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
        /// Datetime must like e.g. 2021-01-30T13:02:11
        /// </summary>
        public static string GetLocalDate(string date)
        {
            string dateStr = date.Split('T')[0];
            var dateCreated = DateTime.Parse(dateStr, CultureInfo.InvariantCulture);
            return dateCreated.ToLocalTime().ToShortDateString();
        }
    }
}