using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Pharmacies.Updates.UpdatePharmacy;

public class UpdatePharmacyCommandHandler : IRequestHandler<UpdatePharmacyCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdatePharmacyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> Handle(UpdatePharmacyCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}