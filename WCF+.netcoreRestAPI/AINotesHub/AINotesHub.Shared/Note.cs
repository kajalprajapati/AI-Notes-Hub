using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.Shared
{
    public class Note
    {
        // Unique identifier for the note
        public Guid Id { get; set; } = Guid.NewGuid();

        // Note title (mandatory, max 100 chars)
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        // Note content (mandatory, max 1000 chars)
        [Required(ErrorMessage = "Content is required")]
        [MaxLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string Content { get; set; } = string.Empty;

        // Category (mandatory, max 50 chars)
        [Required(ErrorMessage = "Category is required")]
        [MaxLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        //public string Category { get; set; } = "General";

        public string Category { get; set; }

        // Creation timestamp (mandatory)
        [Required]
        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Last update timestamp (optional)
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(20)]
        public string CardBackground { get; set; }
        //[MaxLength(20)]
        //public string CardColor { get; set; } = "#FFFFFFFF"; // default white
    }
}
