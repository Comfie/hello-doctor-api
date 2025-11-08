using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Notifications.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Notifications.Queries.GetMyPreferences;

public class GetMyPreferencesHandler : IRequestHandler<GetMyPreferencesQuery, Result<List<NotificationPreferenceResponse>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;

    public GetMyPreferencesHandler(IApplicationDbContext db, IUser user)
    {
        _db = db;
        _user = user;
    }

    public async Task<Result<List<NotificationPreferenceResponse>>> Handle(GetMyPreferencesQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_user.Id))
            return Result<List<NotificationPreferenceResponse>>.Unauthorized();

        var preferences = await _db.NotificationPreferences
            .AsNoTracking()
            .Where(np => np.UserId == _user.Id)
            .Select(np => new NotificationPreferenceResponse(
                np.Id,
                np.NotificationType,
                np.IsEnabled,
                np.PreferredChannel
            ))
            .ToListAsync(ct);

        return Result.Success(preferences);
    }
}
