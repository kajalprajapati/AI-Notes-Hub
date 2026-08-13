using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.WPF.Services
{
    public static class ApiEndpoints
    {

        // Change API version here only
        private const string Version = "v1";

        private const string BasePath = $"api/{Version}";

        // Auth
        public const string Login = $"{BasePath}/auth/login";
        public const string Register = $"{BasePath}/auth/register";

        // Notes
        public const string Notes = $"{BasePath}/notes";
        public const string NotesPaged = $"{BasePath}/notes/paged";

        // Attachments
        public const string Attachments = $"{BasePath}/attachments";

    }
}
