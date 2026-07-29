using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AINotesHub.Shared.Entities;

namespace AINotesHub.Shared.Models
{
    public class NoteAttachment
    {
        public Guid Id { get; set; }

        // Relationship
        public Guid NoteId { get; set; }

        public Note Note { get; set; } = null!;

        // Original file uploaded by user
        public string OriginalFileName { get; set; } = string.Empty;

        // Unique file name stored on server
        public string StoredFileName { get; set; } = string.Empty;

        // uploads/abc123.pdf
        public string FilePath { get; set; } = string.Empty;

        // application/pdf, image/png...
        public string ContentType { get; set; } = string.Empty;

        // .pdf, .png...
        public string FileExtension { get; set; } = string.Empty;

        // Size in bytes
        public long FileSize { get; set; }

        // Audit
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Guid UploadedBy { get; set; }
    }
}
