using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Notifications.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsHandler : IRequestHandler<GetMyNotificationsQuery, Result<List<NotificationResponse>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;

    public GetMyNotificationsHandler(IApplicationDbContext db, IUser user)
    {
        _db = db;
        _user = user;
    }

    public async Task<Result<List<NotificationResponse>>> Handle(GetMyNotificationsQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_user.Id))
            return Result<List<NotificationResponse>>.Unauthorized();

        var query = _db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == _user.Id);

        if (request.UnreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        var notifications = await query
            .OrderByDescending(n => n.SentAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationResponse(
                n.Id,
                n.Type,
                n.Channel,
                n.Subject,
                n.Message,
                n.IsRead,
                n.ReadAt,
                n.SentAt,
                n.PrescriptionId
            ))
            .ToListAsync(ct);

        return Result.Success(notifications);
    }
}
