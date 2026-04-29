using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.WPF.Models
{
    public class Note
    {
        //public int Id { get; set; }
        //public string Title { get; set; }
        //public string Content { get; set; }
        //public DateTime? ReminderTime { get; set; }
        public int Id { get; set; }

        public string? Title { get; set; }

        public string Content { get; set; }

        public DateTime? ReminderTime { get; set; }

        public string? SuggestedTitlesJson { get; set; }   // ✅ ADD THIS

    }
}
