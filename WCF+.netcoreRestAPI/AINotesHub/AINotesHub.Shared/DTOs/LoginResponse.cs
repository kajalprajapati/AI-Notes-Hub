using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.Shared.DTOs
{
    /// <summary>
    /// Represents the response returned after successful login.
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string Role { get; set; } = string.Empty;
        public Guid UserId { get; set; }   // ✅ Add this line
        public string Email { get; set; } = string.Empty;   // ← ADD THIS

    }
}
