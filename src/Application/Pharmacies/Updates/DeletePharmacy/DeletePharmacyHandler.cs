using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;
using Microsoft.Extensions.Caching.Memory;
using Result = Ardalis.Result.Result;

// using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Pharmacies.Updates.DeletePharmacy;

public class DeletePharmacyCommandHandler : IRequestHandler<DeletePharmacyCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;

    public DeletePharmacyCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Result<bool>> Handle(DeletePharmacyCommand request, CancellationToken cancellationToken)
    {
        var pharmacy = await _context
            .Pharmacies
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (pharmacy is null)
        {
            return Result<bool>.NotFound(new Error("Delete Pharmacy", "Pharmacy not found"));
        }

        pharmacy.IsActive = false;
        pharmacy.IsDeleted = true;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate pharmacy caches
        _cache.Remove("AllPharmacies");
        _cache.Remove("ActivePharmacies");

        return Result.Success(true);
    }
}