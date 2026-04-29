using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Serilog;

namespace AINotesHub.WPF.Helpers
{
    public static class ValidationHelper
    {
        // Validate email format
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //public static ShowError(string message)
        //{
        //    txtError.Text = message;
        //    txtError.Visibility = Visibility.Visible;
        //}

        // Simple empty check
        public static bool IsEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        // Validate strong password
        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            // password: min 6 chars, must contain 1 digit
            return Regex.IsMatch(password, @"^(?=.*\d).{6,}$");
        }

        // ✅ Password Strength Calculation (Reusable)
        public static int CalculatePasswordStrength(string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password))
                    return 0;

                int score = 0;

                if (password.Length >= 6) score++;
                if (password.Any(char.IsUpper)) score++;
                if (password.Any(char.IsDigit)) score++;
                if (password.Any(ch => "!@#$%^&*()_+-=[]{}|;:'\",.<>/?".Contains(ch))) score++;

                return score;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Password strength calculation failed.");
                return 0;
            }
        }
    }
}
