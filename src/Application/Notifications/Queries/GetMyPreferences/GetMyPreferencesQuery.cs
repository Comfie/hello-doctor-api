using Ardalis.Result;
using HelloDoctorApi.Application.Notifications.Models;

namespace HelloDoctorApi.Application.Notifications.Queries.GetMyPreferences;

public record GetMyPreferencesQuery : IRequest<Result<List<NotificationPreferenceResponse>>>;
