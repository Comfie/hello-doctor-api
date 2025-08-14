using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Commands.UploadPrescription;

public class UploadPrescriptionCommandHandler : IRequestHandler<UploadPrescriptionCommand, Result<long>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDocumentService _documents;
    private readonly IUser _user;
    private readonly IDateTimeService _clock;

    public UploadPrescriptionCommandHandler(IApplicationDbContext db, IDocumentService documents, IUser user, IDateTimeService clock)
    {
        _db = db;
        _documents = documents;
        _user = user;
        _clock = clock;
    }

    public async Task<Result<long>> Handle(UploadPrescriptionCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_user.Id)) return Result<long>.Unauthorized();

        var beneficiary = await _db.Beneficiaries.Include(b => b.MainMember).ThenInclude(u => u.MainMember)
            .FirstOrDefaultAsync(b => b.Id == request.BeneficiaryId, ct);
        if (beneficiary is null) return Result<long>.NotFound();
        if (beneficiary.MainMemberId != _user.Id) return Result<long>.Forbidden();

        var prescription = new Prescription
        {
            IssuedDate = _clock.OffsetNow,
            ExpiryDate = _clock.OffsetNow.AddMonths(1),
            Status = PrescriptionStatus.Pending,
            Notes = request.Notes,
            MainMemberId = _user.Id!,
            MainMember = beneficiary.MainMember,
            BeneficiaryId = beneficiary.Id,
            Beneficiary = beneficiary
        };

        // auto-assign to preferred pharmacy if none provided
        long? pharmacyId = request.PharmacyId ?? beneficiary.MainMember.MainMember?.DefaultPharmacyId;
        if (pharmacyId.HasValue)
        {
            prescription.AssignToPharmacy(pharmacyId.Value);
        }

        _db.Prescriptions.Add(prescription);
        await _db.SaveChangesAsync(ct);

        return Result.Success(prescription.Id);
    }
}