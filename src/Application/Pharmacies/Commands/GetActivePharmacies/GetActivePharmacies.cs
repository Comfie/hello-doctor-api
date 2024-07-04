using ApiBaseTemplate.Application.Common.Interfaces;

namespace ApiBaseTemplate.Application.Pharmacies.Commands.GetActivePharmacies;

public record GetActivePharmaciesCommand : IRequest<int>
{
}

public class GetActivePharmaciesCommandValidator : AbstractValidator<GetActivePharmaciesCommand>
{
    public GetActivePharmaciesCommandValidator()
    {
    }
}

public class GetActivePharmaciesCommandHandler : IRequestHandler<GetActivePharmaciesCommand, int>
{
    private readonly IApplicationDbContext _context;

    public GetActivePharmaciesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> Handle(GetActivePharmaciesCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
