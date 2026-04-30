using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AINotesHub.WPF.Helpers
{
    //This works offline
    using System.Text.RegularExpressions;

    public static class ReminderParser // A simple parser to extract date/time from text like "Remind me tomorrow" or "Set a reminder for next week"
    {
        public static DateTime? TryParseDate(string text)
        {
            // Try normal date
            if (DateTime.TryParse(text, out var date))
                return date;

            // Tomorrow
            if (text.ToLower().Contains("tomorrow"))
                return DateTime.Now.AddDays(1);

            // Next week
            if (text.ToLower().Contains("next week"))
                return DateTime.Now.AddDays(7);

            // Simple DD/MM/YYYY
            var match = Regex.Match(text, @"\d{1,2}/\d{1,2}/\d{4}");
            if (match.Success)
            {
                if (DateTime.TryParse(match.Value, out var parsed))
                    return parsed;
            }

            return null;
        }
    }


}
