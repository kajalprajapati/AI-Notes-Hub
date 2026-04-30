using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.Shared.Entities
{
    public class AppUser : BaseEntity
    {
        // 🔑 Primary key — required by EF Core for identity tracking
        //[Key]
        //public Guid Id { get; set; } = Guid.NewGuid();

        //[Required(ErrorMessage = "Username is required.")]
        //[MaxLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        public string Username { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Password hash is required.")]
        //[MaxLength(256, ErrorMessage = "Password hash length exceeded.")]
        public string PasswordHash { get; set; } = string.Empty;
        //public string Password { get; set; } = string.Empty;

        // 🎭 Role — required for role-based access (Admin/User/Manager)
        //[Required(ErrorMessage = "Role is required.")]
        //[MaxLength(50, ErrorMessage = "Role cannot exceed 50 characters.")]
        public string Role { get; set; } = "User";

        //[Required(ErrorMessage = "Email is required.")]
        //[EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;  // ✅ NEW FIELD

        //public DateTime CreatedAt { get; set; }

        // 🔗 One-to-many relationship: One user can have many notes
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
