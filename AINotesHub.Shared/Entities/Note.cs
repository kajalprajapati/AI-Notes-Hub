using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AINotesHub.Shared.Entities
{
    public partial class Note : BaseEntity
    {
        public string? Title { get; set; }

        public string Content { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string CardBackground { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public AppUser? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ReminderDateTime { get; set; }

        public bool IsReminderOn { get; set; }

        public string? SuggestedTitlesJson { get; set; }  // ✅ Your new field

        public string? AttachmentFileName { get; set; }

        public string? AttachmentPath { get; set; }

        // 🆕 Added with annotation
        
        //public string category = string.Empty;

        //// Creation timestamp (mandatory)
        ////[Required]
        ////public DateTime CreatedAt { get; set; } = DateTime.Now;

        //// Last update timestamp (optional)
        ////public DateTime? UpdatedAt { get; set; }

        ////[MaxLength(20)]
        //[ObservableProperty]
        //public string cardBackground = string.Empty;

        ////[Required(ErrorMessage = "Creator information is required.")]
        ////[StringLength(50, ErrorMessage = "Creator name cannot exceed 50 characters.")]
        
    }
}
