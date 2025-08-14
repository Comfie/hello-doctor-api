using Ardalis.Result;

namespace HelloDoctorApi.Application.Prescriptions.Queries.GetPharmacyPrescriptions;

public record GetPharmacyPrescriptionsQuery(long? PharmacyId = null) : IRequest<Result<List<GetPharmacyPrescriptionsResult>>>;