using Ardalis.Result;

namespace HelloDoctorApi.Application.Reporting.Queries.GetMyRecentActivity;

public record GetMyRecentActivityQuery(int Limit = 20) : IRequest<Result<List<MyRecentActivityItem>>>;

public record MyRecentActivityItem(long PrescriptionId, string OldStatus, string NewStatus, DateTimeOffset ChangedDate, string Reason);

