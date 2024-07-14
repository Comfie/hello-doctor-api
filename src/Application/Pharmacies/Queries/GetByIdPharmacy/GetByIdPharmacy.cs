using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Application.Pharmacies.Models;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Pharmacies.Queries.GetByIdPharmacy;

public record GetByIdPharmacyCommand(long Id) : IRequest<Result<PharmacyResponse>>;

public class GetByIdPharmacyCommandValidator : AbstractValidator<GetByIdPharmacyCommand>
{
    public GetByIdPharmacyCommandValidator()
    {
    }
}

public class GetByIdPharmacyCommandHandler : IRequestHandler<GetByIdPharmacyCommand, Result<PharmacyResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetByIdPharmacyCommandHandler(IApplicationDbContext context)
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
            return Result.Failure<PharmacyResponse>(new Error("Get Pharmacy", "Pharmacy not found"));
        }

        return Result.Success(pharmacy);
    }
}