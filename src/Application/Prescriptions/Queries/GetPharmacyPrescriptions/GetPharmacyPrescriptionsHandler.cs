using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Prescriptions.Queries.GetPharmacyPrescriptions;

public class GetPharmacyPrescriptionsQueryHandler : IRequestHandler<GetPharmacyPrescriptionsQuery, bool>
{
    private readonly IApplicationDbContext _context;

    public GetPharmacyPrescriptionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> Handle(GetPharmacyPrescriptionsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}