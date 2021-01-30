using System;

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
    }
}