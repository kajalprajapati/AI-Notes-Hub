using AINotesHub.API.Data;
using AINotesHub.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AINotesHub.API.Services
{
    [Authorize]
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReminderBackgroundService> _logger;

        public ReminderBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<ReminderBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var context = scope.ServiceProvider
                        .GetRequiredService<NotesDbContext>();

                    var notificationService = scope.ServiceProvider
                        .GetRequiredService<NotificationService>();

                    var now = DateTime.UtcNow;

                    var localDate = DateTime.Now;


                    var dueReminders = await context.Notes
                        .AsNoTracking()
                        .Where(n =>
                            n.IsReminderOn &&
                            n.ReminderDateTime != null &&
                            n.ReminderDateTime <= now)
                        .ToListAsync(stoppingToken);

                    foreach (var note in dueReminders)
                    {

                        var notificationExists = await context.Notifications
               .AnyAsync(n =>
                   n.UserId == note.UserId &&
                   n.NoteId == note.Id);

                        if (notificationExists)
                        {
                            continue;
                        }

                        await notificationService.CreateNotificationAsync(
                           note.UserId,
                           note.Id,
                           $"Reminder: {note.Title}");

                        _logger.LogInformation(
                        "Reminder processed for Note {NoteId}",
                        note.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error while processing reminders");
                }

                await Task.Delay(
                    TimeSpan.FromMinutes(1),
                    stoppingToken);
            }
        }
    }
}