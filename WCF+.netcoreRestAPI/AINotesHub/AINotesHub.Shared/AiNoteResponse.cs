using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.Shared
{
    public class AiNoteResponse
    {
        public List<string> Titles { get; set; } = new();
        public string Summary { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
    }
}
