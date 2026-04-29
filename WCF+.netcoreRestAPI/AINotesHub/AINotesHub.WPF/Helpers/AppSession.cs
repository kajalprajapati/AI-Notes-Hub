using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AINotesHub.WPF.Properties;
using AINotesHub.WPF.Services;
using AINotesHub.WPF.ViewModels;

namespace AINotesHub.WPF.Helpers
{
    /// <summary>
    /// ⭐ SESSION MANAGER
    /// Stores global session data such as JWT token and current user.
    /// SRP (Single Responsibility Principle).
    /// One class manages everything related to user session
    //(Token, Username, Email, Role, Expiry, RememberMe, Logout)
    /// </summary>
    public static class AppSession
    {
        //✔ This class stores user login information:

        public static NotesApiService NotesService { get; set; }
        public static ColorStateViewModel ColorState { get; } = new();
        public static string JwtToken { get; set; } = string.Empty;
        public static string Username { get; set; } = string.Empty;
        public static string Email { get; set; } = string.Empty;     // ✅ Added
        public static string Role { get; set; } = string.Empty;
        public static Guid UserId { get; set; }   // ✅ Add this
        public static bool IsAuthenticated => !string.IsNullOrEmpty(JwtToken);
        public static DateTime ExpiryTime { get; set; }
        //public static bool IsExpired => DateTime.UtcNow > ExpiryTime;
        public static bool IsExpired => DateTime.Now > ExpiryTime;
        public static bool IsLoggedIn =>
            !string.IsNullOrEmpty(JwtToken) && !IsExpired;
        public static async Task Clear()
        {
            JwtToken = string.Empty;
            Username = string.Empty;
            Email = string.Empty;
            Role = string.Empty;
            UserId = Guid.Empty;
            ExpiryTime = DateTime.MinValue;


            // 🔹 Clear persistent saved settings
            Settings.Default.Username = string.Empty;
            Settings.Default.Role = string.Empty;
            Settings.Default.Email = string.Empty;
            Settings.Default.JwtToken = string.Empty;
            Settings.Default.UserId = string.Empty;
            Settings.Default.ProfileImagePath = string.Empty;// Clear saved profile image
            Settings.Default.ExpiryTime = DateTime.MinValue; // Clear session expiry time
            Settings.Default.RememberMe = false;    // ⭐ Reset this also! Clear RememberMe flag


            Settings.Default.Save();// 🔹 Save changes

            // Optionally delete cached image file
            string savedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AINotesHub", "profile.jpg");

            if (File.Exists(savedPath))
                File.Delete(savedPath);

        }

    }
}
