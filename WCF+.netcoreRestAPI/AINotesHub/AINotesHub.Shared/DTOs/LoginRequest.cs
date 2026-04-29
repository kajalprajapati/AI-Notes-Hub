using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.Shared.DTOs
{
    /// <summary>
    /// Represents the login request sent from client to API.
    /// </summary>
    /// ///DTOs Data Transfer Objects -Purpose: Represent API input/output
    public class LoginRequest
    {
       

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Email is required.")]
        //[EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [Required(ErrorMessage = "Email or Username is required.")]
        public string UsernameOrEmail { get; set; }
    }
}
