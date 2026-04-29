
using CommunityToolkit.Mvvm.ComponentModel;

namespace AINotesHub.Shared.Entities
{
    public abstract class BaseEntity : ObservableObject
    {
        //abstract class :-  It contains common properties used by all entities

        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        // Soft delete flag
        public bool IsArchived { get; set; }   // true if archived
        public bool IsDeleted { get; set; }    // true if trashed
        public bool IsStarred { get; set; }
        public bool IsImportant { get; set; }


    }
}
