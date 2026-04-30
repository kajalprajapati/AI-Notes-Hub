using AINotesHub.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace AINotesHub.API.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Username = "admin",
                    PasswordHash = "$2a$11$KbQiCKvZ8N7C0Ypyg7xk9Om4uCExbQkufwvxLDtGkXMVE7l5cOK0e",
                    Role = "Admin"
                },
                new AppUser
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Username = "manager",
                    PasswordHash = "$2a$11$2CC7oXv.nDRToKuA3AMUEuMN9oj4oOYb3D5KTebQQCZZfUfGzTtuS",
                    Role = "Manager"
                },
                new AppUser
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Username = "user",
                    PasswordHash = "$2a$11$Mqt3ddZo0Q4wqVJR4s5rK.QwYyyb5QdqvTKhZP4JjzBywA.yEBgDC",
                    Role = "User"
                }
            );
        }
    }
}
