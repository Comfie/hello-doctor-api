using HelloDoctorApi.Application.Common.Interfaces;

namespace HelloDoctorApi.Application.Prescriptions.Commands.UploadPrescription;

public class UploadPrescriptionCommandHandler : IRequestHandler<UploadPrescriptionCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IDocumentService _documentService;

    public UploadPrescriptionCommandHandler(IApplicationDbContext context, IDocumentService documentService)
    {
        _context = context;
        _documentService = documentService;
    }

    public Task<bool> Handle(UploadPrescriptionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}