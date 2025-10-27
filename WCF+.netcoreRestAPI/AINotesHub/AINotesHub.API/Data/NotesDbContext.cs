using Microsoft.EntityFrameworkCore;
using AINotesHub.Shared;   // ✅ Import the Note model namespace


namespace AINotesHub.API.Data
{
    public class NotesDbContext : DbContext
    {
        public NotesDbContext(DbContextOptions<NotesDbContext> options)
        : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; } = null!;
    }
}
