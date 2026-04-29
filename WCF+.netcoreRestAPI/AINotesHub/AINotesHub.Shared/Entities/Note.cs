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

        // 🆕 Added with annotation
        // Unique identifier for the note
        //public Guid Id { get; set; } = Guid.NewGuid();

        // Note title (mandatory, max 100 chars)
        //[Required(ErrorMessage = "Title is required")]
        //[MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        //[ObservableProperty]
        //private string? title;
        //public string? Title { get; set; }
        ////private string title = string.Empty;

        //// Note content (mandatory, max 1000 chars)
        ////[Required(ErrorMessage = "Content is required")]
        ////[MaxLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        //[ObservableProperty]

        //public string content = string.Empty;

        //// Category (mandatory, max 50 chars)
        ////[Required(ErrorMessage = "Category is required")]
        ////[MaxLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        ////public string Category { get; set; } = "General";
        //[ObservableProperty]

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
        ////public string CreatedBy { get; set; } = string.Empty;

        //// 🔗 Relationship
        ////[Required(ErrorMessage = "UserId is required.")]
        //[ObservableProperty]

        //public Guid userId;

        //[ObservableProperty]
        //// foreign key
        //public AppUser? user;        // navigation property
        ////public bool IsArchived { get; set; }   // ✅ ADD
        ////public bool IsDeleted { get; set; } 
        //// ✅ ADD (Trash
        //[ObservableProperty]
        //public DateTime createdAt = DateTime.Now;

        //// 🔔 Reminder Fields
        //[ObservableProperty]

        //public DateTime? reminderDateTime;
        ////public bool IsReminderOn { get; set; }
        //[ObservableProperty]

        //public bool isReminderOn = false;

        ////[ObservableProperty]
        //public string? SuggestedTitlesJson { get; set; }
        //private ObservableCollection<string> suggestedTitles = new();
    }
}
