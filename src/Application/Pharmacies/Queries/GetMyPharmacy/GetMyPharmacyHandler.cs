using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Pharmacies.Models;
using HelloDoctorApi.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Pharmacies.Queries.GetMyPharmacy;

public class GetMyPharmacyHandler : IRequestHandler<GetMyPharmacyQuery, Result<PharmacyResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetMyPharmacyHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<Result<PharmacyResponse>> Handle(GetMyPharmacyQuery request,
        CancellationToken cancellationToken)
    {
        // Get the current user's ID
        var userId = _user.Id;

        if (string.IsNullOrEmpty(userId))
        {
            return Result<PharmacyResponse>.NotFound(new Error("Pharmacy", "User not authenticated"));
        }

        // Find the Pharmacist record for this user
        var pharmacist = await _context.Pharmacists
            .FirstOrDefaultAsync(p => p.AccountId == userId, cancellationToken);

        if (pharmacist is null)
        {
            return Result<PharmacyResponse>.NotFound(new Error("Pharmacy", "User is not associated with any pharmacy"));
        }

        // Fetch the pharmacy using the pharmacist's pharmacy ID
        var pharmacy = await _context.Pharmacies
            .FirstOrDefaultAsync(p => p.Id == pharmacist.PharmacyId && !p.IsDeleted, cancellationToken);

        if (pharmacy is null)
        {
            return Result<PharmacyResponse>.NotFound(new Error("Pharmacy", "Pharmacy not found"));
        }

        return Result.Success(new PharmacyResponse
        {
            Id = pharmacy.Id,
            Name = pharmacy.Name,
            Description = pharmacy.Description,
            ContactNumber = pharmacy.ContactNumber,
            ContactEmail = pharmacy.ContactEmail,
            ContactPerson = pharmacy.ContactPerson,
            Address = pharmacy.Address,
            OpeningTime = pharmacy.OpeningTime,
            ClosingTime = pharmacy.ClosingTime,
            IsActive = pharmacy.IsActive
        });
    }
}
