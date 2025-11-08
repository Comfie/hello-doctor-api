using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Pharmacies.Models;
using HelloDoctorApi.Domain.Shared;
using Microsoft.Extensions.Caching.Memory;

namespace HelloDoctorApi.Application.Pharmacies.Queries.GetAllPharmacies;

public class GetAllPharmaciesCommandHandler : IRequestHandler<GetAllPharmaciesCommand, Result<List<PharmacyResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "AllPharmacies";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public GetAllPharmaciesCommandHandler(IApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<List<PharmacyResponse>>> Handle(GetAllPharmaciesCommand request,
        CancellationToken cancellationToken)
    {
        // Try to get from cache first
        if (_cache.TryGetValue(CacheKey, out List<PharmacyResponse>? cachedPharmacies) && cachedPharmacies != null)
        {
            return Result.Success(cachedPharmacies);
        }

        // Not in cache, fetch from database
        var pharmacies = await _context
            .Pharmacies
            .AsNoTracking()
            .Where(x => x.IsDeleted == false)
            .Select(pharmacy => new PharmacyResponse()
            {
                Id = pharmacy.Id,
                Name = pharmacy.Name,
                Description = pharmacy.Description,
                ContactEmail = pharmacy.ContactEmail,
                ContactNumber = pharmacy.ContactNumber,
                ContactPerson = pharmacy.ContactPerson,
                Address = pharmacy.Address,
                OpeningTime = pharmacy.OpeningTime,
                ClosingTime = pharmacy.ClosingTime,
                IsActive = pharmacy.IsActive
            })
            .ToListAsync(cancellationToken);

        // Store in cache
        _cache.Set(CacheKey, pharmacies, CacheDuration);

        return Result.Success(pharmacies);
    }
}