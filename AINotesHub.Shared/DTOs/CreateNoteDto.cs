using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.Shared.DTOs
{
    public class CreateNoteDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public string CardBackground { get; set; }
        public bool IsStarred { get; set; }
        public bool IsImportant { get; set; }
        public bool IsReminderOn { get; set; }
        public DateTime? ReminderDateTime { get; set; }

    }
}
