using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Prescriptions.Commands.DownloadPrescription;

public class DownloadPrescriptionCommandHandler : IRequestHandler<DownloadPrescriptionCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DownloadPrescriptionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> Handle(DownloadPrescriptionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}