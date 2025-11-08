using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Pharmacies.Models;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HelloDoctorApi.Application.Pharmacies.Updates.TogglePharmacyActive;

public class TogglePharmacyActiveHandler : IRequestHandler<TogglePharmacyActiveCommand, Result<PharmacyResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;

    public TogglePharmacyActiveHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Result<PharmacyResponse>> Handle(TogglePharmacyActiveCommand request,
        CancellationToken cancellationToken)
    {
        // Fetch the pharmacy
        var pharmacy = await _context.Pharmacies
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (pharmacy is null)
        {
            return Result<PharmacyResponse>.NotFound(new Error("Pharmacy", "Pharmacy not found"));
        }

        // Toggle the IsActive status
        pharmacy.IsActive = !pharmacy.IsActive;

        _context.Pharmacies.Update(pharmacy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate pharmacy caches
        _cache.Remove("AllPharmacies");
        _cache.Remove("ActivePharmacies");

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
