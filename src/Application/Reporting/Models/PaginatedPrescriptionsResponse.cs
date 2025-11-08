namespace HelloDoctorApi.Application.Reporting.Models;

public record PaginatedPrescriptionsResponse(
    List<PrescriptionListItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);
