using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AINotesHub.Shared.DTOs
{
    public class UserListDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

    }
}
