using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Prescriptions.Commands.CreatePrescriptionFromUpload;

public class CreatePrescriptionFromUploadHandler : IRequestHandler<CreatePrescriptionFromUploadCommand, Result<long>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;
    private readonly IDateTimeService _clock;

    public CreatePrescriptionFromUploadHandler(IApplicationDbContext db, IUser user, IDateTimeService clock)
    { _db = db; _user = user; _clock = clock; }

    public async Task<Result<long>> Handle(CreatePrescriptionFromUploadCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_user.Id)) return Result<long>.Unauthorized();

        var beneficiary = await _db.Beneficiaries.Include(b => b.MainMember).FirstOrDefaultAsync(b => b.Id == request.BeneficiaryId, ct);
        if (beneficiary is null) return Result<long>.NotFound();
        if (beneficiary.MainMemberId != _user.Id) return Result<long>.Forbidden();

        var file = await _db.FileUploads.FirstOrDefaultAsync(f => f.Id == request.FileUploadId, ct);
        if (file is null) return Result<long>.NotFound();

        var prescription = new Prescription
        {
            IssuedDate = _clock.OffsetNow,
            ExpiryDate = _clock.OffsetNow.AddMonths(1),
            Status = PrescriptionStatus.Pending,
            Notes = request.Notes,
            MainMemberId = _user.Id!,
            MainMember = beneficiary.MainMember,
            BeneficiaryId = beneficiary.Id,
            Beneficiary = beneficiary,
            PrescriptionFiles = new List<FileUpload> { file }
        };

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

