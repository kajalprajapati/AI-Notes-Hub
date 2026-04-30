using AINotesHub.Shared.Entities;   // ✅ Import the Note model namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AINotesHub.API.Data
{
    public class NotesDbContext : DbContext
    {
        public NotesDbContext(DbContextOptions<NotesDbContext> options)
        : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; } = null!;
        public DbSet<AppUser> Users { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            DatabaseSeeder.Seed(modelBuilder);
            // Relationships
            modelBuilder.Entity<Note>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        public override Task<int> SaveChangesAsync(
    bool acceptAllChangesOnSuccess,
    CancellationToken cancellationToken = default)
        {

            //            This triggers ON EVERY SAVE
            //✔ Automatically sets timestamps
            //✔ Zero change in controller
            //✔ Industry - standard approach
            foreach (var entry in ChangeTracker.Entries<Note>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    // entity.CreatedBy = currentUserId;   // if using auth
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

    }
}
