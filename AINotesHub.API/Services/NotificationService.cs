using AINotesHub.API.Data;
using AINotesHub.Shared.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AINotesHub.API.Services
{
    public class NotificationService
    {
        private readonly NotesDbContext _context;

        public NotificationService(NotesDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(
            Guid userId,
            Guid noteId,
            string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                NoteId = noteId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };


            //CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
        }
    }
}
