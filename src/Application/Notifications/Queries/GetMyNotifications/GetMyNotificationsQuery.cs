using Ardalis.Result;
using HelloDoctorApi.Application.Notifications.Models;

namespace HelloDoctorApi.Application.Notifications.Queries.GetMyNotifications;

public record GetMyNotificationsQuery(
    bool UnreadOnly = false,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<List<NotificationResponse>>>;
