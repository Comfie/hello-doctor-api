using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Application.Pharmacies.Models;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Pharmacies.Queries.GetAllPharmacies;

public record GetAllPharmaciesCommand : IRequest<Result<List<PharmacyResponse>>>;

public class GetAllPharmaciesCommandValidator : AbstractValidator<GetAllPharmaciesCommand>
{
    public GetAllPharmaciesCommandValidator()
    {
    }
}

public class GetAllPharmaciesCommandHandler : IRequestHandler<GetAllPharmaciesCommand, Result<List<PharmacyResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllPharmaciesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PharmacyResponse>>> Handle(GetAllPharmaciesCommand request, CancellationToken cancellationToken)
    {
        var pharmacies = await _context
            .Pharmacies
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
        return Result.Success(pharmacies);
    }
}
