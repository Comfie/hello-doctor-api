using ApiBaseTemplate.Application.Common.Interfaces;

namespace ApiBaseTemplate.Application.Pharmacies.Commands.GetByIdPharmacy;

public record GetByIdPharmacyCommand : IRequest<int>
{
}

public class GetByIdPharmacyCommandValidator : AbstractValidator<GetByIdPharmacyCommand>
{
    public GetByIdPharmacyCommandValidator()
    {
    }
}

public class GetByIdPharmacyCommandHandler : IRequestHandler<GetByIdPharmacyCommand, int>
{
    private readonly IApplicationDbContext _context;

    public GetByIdPharmacyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> Handle(GetByIdPharmacyCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
