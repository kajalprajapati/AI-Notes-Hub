using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace AINotesHub.Shared.DTOs
{
    public class FileUploadRequest
    {
        [Required]
        public Guid NoteId { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
