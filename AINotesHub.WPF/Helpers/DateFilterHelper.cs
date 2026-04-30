using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.WPF.Helpers
{
    static class DateFilterHelper
    {
        /// <summary>
        /// Checks if a date is today.
        /// </summary>
        public static bool IsToday(DateTime date)
        {
            DateTime today = DateTime.Today;
            return date.Date == today;
        }

        /// <summary>
        /// Checks if a date is in the current week.
        /// Week starts on Monday.
        /// </summary>
        public static bool IsThisWeek(DateTime date)
        {
            DateTime today = DateTime.Today;
            DateTime startOfWeek = GetStartOfWeek(today);
            DateTime endOfWeek = startOfWeek.AddDays(6);
            return date.Date >= startOfWeek && date.Date <= endOfWeek;
        }

        /// <summary>
        /// Checks if a date is in the next week.
        /// </summary>
        public static bool IsNextWeek(DateTime date)
        {
            DateTime today = DateTime.Today;
            DateTime startOfNextWeek = GetStartOfWeek(today).AddDays(7);
            DateTime endOfNextWeek = startOfNextWeek.AddDays(6);
            return date.Date >= startOfNextWeek && date.Date <= endOfNextWeek;
        }

        /// <summary>
        /// Checks if a date is in the current month.
        /// </summary>
        public static bool IsThisMonth(DateTime date)
        {
            DateTime today = DateTime.Today;
            return date.Month == today.Month && date.Year == today.Year;
        }

        /// <summary>
        /// Checks if a date is in the next month.
        /// </summary>
        public static bool IsNextMonth(DateTime date)
        {
            DateTime today = DateTime.Today.AddMonths(1);
            return date.Month == today.Month && date.Year == today.Year;
        }

        /// <summary>
        /// Helper: Get Monday of a given date's week.
        /// </summary>
        public static DateTime GetStartOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
            return date.AddDays(-diff).Date;
        }
    }
}
