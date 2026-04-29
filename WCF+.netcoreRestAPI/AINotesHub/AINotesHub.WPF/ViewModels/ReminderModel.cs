using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.WPF.ViewModels
{
    public class ReminderModel
    {
        public int NoteId { get; set; }

        public DateTime ReminderTime { get; set; }

        public string Message { get; set; }

        public bool IsCompleted { get; set; }
    }
}
