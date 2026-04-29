using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AINotesHub.WPF.Helpers;
using Windows.Services.Maps;

namespace AINotesHub.WPF.Services
{
    public class ReminderService
    {
        private readonly AIService _aiService;
        public ReminderService(AIService aiService)
        {
            _aiService = aiService;
        }

        //public async Task<DateTime?> GetReminderAsync(string noteText)
        //{
        public async Task<DateTime?> GetReminderAsync(string text)
        {
            // For now: simple rule-based (offline + fast)

            return await Task.Run(() =>
            {
                // Example: "birthday on 15 Feb"
                var match = Regex.Match(
                    text,
                    @"(\d{1,2})\s?(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)",
                    RegexOptions.IgnoreCase);

                if (!match.Success)
                    return (DateTime?)null;

                int day = int.Parse(match.Groups[1].Value);
                string monthText = match.Groups[2].Value;

                int month = DateTime.ParseExact(
                    monthText,
                    "MMM",
                    null).Month;

                return new DateTime(
                    DateTime.Now.Year,
                    month,
                    day);
            });
        }
        //// 1️⃣ Try without AI
        //var local = ReminderParser.TryParseDate(noteText);

        //if (local != null)
        //    return local;

        //// 2️⃣ Use AI if needed
        //return await _aiService.ExtractReminderDateAsync(noteText);
    }

}

