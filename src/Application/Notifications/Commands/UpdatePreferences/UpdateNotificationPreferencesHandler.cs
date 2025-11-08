using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Notifications.Commands.UpdatePreferences;

public class UpdateNotificationPreferencesHandler : IRequestHandler<UpdateNotificationPreferencesCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;

    public UpdateNotificationPreferencesHandler(IApplicationDbContext db, IUser user)
    {
        _db = db;
        _user = user;
    }

    public async Task<Result> Handle(UpdateNotificationPreferencesCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_user.Id))
            return Result.Unauthorized();

        var userId = _user.Id;

        // Get existing preferences for this user
        var existingPreferences = await _db.NotificationPreferences
            .Where(np => np.UserId == userId)
            .ToListAsync(ct);

        foreach (var update in request.Preferences)
        {
            var existing = existingPreferences
                .FirstOrDefault(p => p.NotificationType == update.NotificationType);

            if (existing != null)
            {
                // Update existing preference
                existing.IsEnabled = update.IsEnabled;
                existing.PreferredChannel = update.PreferredChannel;
            }
            else
            {
                // Create new preference
                var user = await _db.ApplicationUsers
                    .FirstOrDefaultAsync(u => u.Id == userId, ct);

                if (user == null)
                    return Result.Unauthorized();

                var newPreference = new NotificationPreference
                {
                    UserId = userId,
                    User = user,
                    NotificationType = update.NotificationType,
                    IsEnabled = update.IsEnabled,
                    PreferredChannel = update.PreferredChannel
                };

                _db.NotificationPreferences.Add(newPreference);
            }
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
