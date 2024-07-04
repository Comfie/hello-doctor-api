using ApiBaseTemplate.Application.Common.Interfaces;

namespace ApiBaseTemplate.Application.Pharmacies.Commands.GetAllPharmacies;

public record GetAllPharmaciesCommand : IRequest<int>
{
}

public class GetAllPharmaciesCommandValidator : AbstractValidator<GetAllPharmaciesCommand>
{
    public GetAllPharmaciesCommandValidator()
    {
    }
}

public class GetAllPharmaciesCommandHandler : IRequestHandler<GetAllPharmaciesCommand, int>
{
    private readonly IApplicationDbContext _context;

    public GetAllPharmaciesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> Handle(GetAllPharmaciesCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
