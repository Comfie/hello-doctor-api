using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Pharmacies.Models;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Pharmacies.Queries.GetActivePharmacies;

public record GetActivePharmaciesCommand : IRequest<Result<List<PharmacyResponse>>>;

public class GetActivePharmaciesCommandValidator : AbstractValidator<GetActivePharmaciesCommand>
{
    public GetActivePharmaciesCommandValidator()
    {
    }
}

public class
    GetActivePharmaciesCommandHandler : IRequestHandler<GetActivePharmaciesCommand, Result<List<PharmacyResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetActivePharmaciesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PharmacyResponse>>> Handle(GetActivePharmaciesCommand request,
        CancellationToken cancellationToken)
    {
        var pharmacies = await _context
            .Pharmacies
            .Where(x => x.IsActive == true && x.IsDeleted == false)
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
        return Result.Success(pharmacies);
    }
}