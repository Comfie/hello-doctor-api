using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Notifications.Updates.MarkAsRead;

public class MarkNotificationAsReadHandler : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;

    public MarkNotificationAsReadHandler(IApplicationDbContext db, IUser user)
    {
        _db = db;
        _user = user;
    }

    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_user.Id))
            return Result.Unauthorized();

        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.UserId == _user.Id, ct);

        if (notification == null)
            return Result.NotFound();

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        return Result.Success();
    }
}
