using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Pharmacies.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Pharmacies.Queries.GetByIdPharmacy;

public class GetByIdPharmacyHandler : IRequestHandler<GetByIdPharmacyCommand, Result<PharmacyResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetByIdPharmacyHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PharmacyResponse>> Handle(GetByIdPharmacyCommand request,
        CancellationToken cancellationToken)
    {
        var pharmacy = await _context
            .Pharmacies
            .Where(x => x.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (pharmacy is null)
        {
            return Result<PharmacyResponse>.NotFound(new Error("Get Pharmacy", "Pharmacy not found"));
        }

        return Result.Success(pharmacy);
    }
}