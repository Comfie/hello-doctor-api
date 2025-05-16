using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Prescriptions.Queries.GetPrescriptions;

public class GetPrescriptionsQueryHandler : IRequestHandler<GetPrescriptionsQuery, bool>
{
    private readonly IApplicationDbContext _context;

    public GetPrescriptionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> Handle(GetPrescriptionsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}