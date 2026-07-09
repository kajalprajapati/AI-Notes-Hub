using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.Shared.DTOs
{
    public class UpdateNoteDto
    {
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string CardBackground { get; set; } = string.Empty;

    }
}
