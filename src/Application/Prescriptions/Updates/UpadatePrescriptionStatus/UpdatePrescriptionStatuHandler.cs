using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Prescriptions.Updates.UpadatePrescriptionStatus;

public class UpadatePrescriptionStatusCommandHandler : IRequestHandler<UpdatePrescriptionStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpadatePrescriptionStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> Handle(UpdatePrescriptionStatusCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}