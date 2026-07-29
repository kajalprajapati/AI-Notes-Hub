using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AINotesHub.API.Data
{
    //for migrations
    public class NotesDbContextFactory : IDesignTimeDbContextFactory<NotesDbContext>
    {
        public NotesDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NotesDbContext>();

            optionsBuilder.UseSqlServer(
                "Data Source=(localdb)\\MSSQLLocalDB;" +
                "Initial Catalog=AINotesHubDB;" +
                "Integrated Security=True;" +
                "Encrypt=False;" +
                "TrustServerCertificate=True;");

            return new NotesDbContext(optionsBuilder.Options);
        }
    }
}
