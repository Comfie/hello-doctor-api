using ApiBaseTemplate.Application.Common.Interfaces;

namespace ApiBaseTemplate.Application.Pharmacies.Commands.CreatePharmacy;

public record CreatePharmacyCommand : IRequest<int>
{
}

public class CreatePharmacyCommandValidator : AbstractValidator<CreatePharmacyCommand>
{
    public CreatePharmacyCommandValidator()
    {
    }
}

public class CreatePharmacyCommandHandler : IRequestHandler<CreatePharmacyCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreatePharmacyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public  Task<int> Handle(CreatePharmacyCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
