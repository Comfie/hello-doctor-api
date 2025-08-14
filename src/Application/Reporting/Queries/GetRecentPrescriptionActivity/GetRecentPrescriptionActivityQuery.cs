using Ardalis.Result;

namespace HelloDoctorApi.Application.Reporting.Queries.GetRecentPrescriptionActivity;

public record GetRecentPrescriptionActivityQuery(long PharmacyId, int Limit = 20) : IRequest<Result<List<RecentActivityItem>>>;

public record RecentActivityItem(long PrescriptionId, string OldStatus, string NewStatus, DateTimeOffset ChangedDate, string Reason);
