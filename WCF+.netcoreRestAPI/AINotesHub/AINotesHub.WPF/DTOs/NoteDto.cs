using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.WPF.DTOs
{
    public class NoteDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        // Extra UI-only fields:
        public bool IsSelected { get; set; }
        public bool IsEditing { get; set; }

    }
}
