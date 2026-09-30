using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.Shared.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }

        public Guid NoteId { get; set; }

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } 

        public AppUser  ? User { get; set; }

        public Note? Note { get; set; }
    }
}
