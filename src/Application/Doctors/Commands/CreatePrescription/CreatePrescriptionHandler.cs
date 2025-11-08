using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Doctors.Commands.CreatePrescription;

public class CreatePrescriptionHandler : IRequestHandler<CreatePrescriptionCommand, Result<long>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUser _user;

    public CreatePrescriptionHandler(IApplicationDbContext db, IUser user)
    {
        _db = db;
        _user = user;
    }

    public async Task<Result<long>> Handle(CreatePrescriptionCommand request, CancellationToken ct)
    {
        // Verify doctor is authenticated
        if (string.IsNullOrWhiteSpace(_user.Id))
            return Result<long>.Unauthorized();

        var doctorId = _user.GetDoctorId();
        if (!doctorId.HasValue)
            return Result<long>.Forbidden();

        // Verify beneficiary exists
        var beneficiary = await _db.Beneficiaries
            .Include(b => b.MainMember)
                .ThenInclude(u => u.MainMember)
            .FirstOrDefaultAsync(b => b.Id == request.BeneficiaryId && !b.IsDeleted, ct);

        if (beneficiary is null)
            return Result<long>.NotFound("Beneficiary not found");

        // Get the main member
        var mainMember = await _db.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Id == beneficiary.MainMemberId, ct);

        if (mainMember is null)
            return Result<long>.NotFound("Main member not found");

        // Create the prescription
        var prescription = new Prescription
        {
            IssuedDate = request.IssuedDate,
            ExpiryDate = request.ExpiryDate,
            Status = PrescriptionStatus.Pending,
            Notes = request.Notes,
            MainMemberId = beneficiary.MainMemberId,
            MainMember = mainMember,
            BeneficiaryId = beneficiary.Id,
            Beneficiary = beneficiary,
            DoctorId = doctorId.Value
        };

        // Assign to pharmacy if specified
        if (request.PharmacyId.HasValue)
        {
            var pharmacy = await _db.Pharmacies
                .FirstOrDefaultAsync(p => p.Id == request.PharmacyId.Value && p.IsActive && !p.IsDeleted, ct);

            if (pharmacy is null)
                return Result<long>.NotFound("Pharmacy not found or inactive");

            prescription.AssignToPharmacy(request.PharmacyId.Value);
        }

        _db.Prescriptions.Add(prescription);
        await _db.SaveChangesAsync(ct);

        return Result.Success(prescription.Id);
    }
}
