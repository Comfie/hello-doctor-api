using ApiBaseTemplate.Application.Common.Interfaces;

namespace ApiBaseTemplate.Application.Pharmacies.Commands.UpdatePharmacy;

public record UpdatePharmacyCommand : IRequest<int>
{
}

public class UpdatePharmacyCommandValidator : AbstractValidator<UpdatePharmacyCommand>
{
    public UpdatePharmacyCommandValidator()
    {
    }
}

public class UpdatePharmacyCommandHandler : IRequestHandler<UpdatePharmacyCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdatePharmacyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> Handle(UpdatePharmacyCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
