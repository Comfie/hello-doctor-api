using ApiBaseTemplate.Application.Common.Interfaces;

namespace ApiBaseTemplate.Application.Pharmacies.Commands.DeletePharmacy;

public record DeletePharmacyCommand : IRequest<int>
{
}

public class DeletePharmacyCommandValidator : AbstractValidator<DeletePharmacyCommand>
{
    public DeletePharmacyCommandValidator()
    {
    }
}

public class DeletePharmacyCommandHandler : IRequestHandler<DeletePharmacyCommand, int>
{
    private readonly IApplicationDbContext _context;

    public DeletePharmacyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> Handle(DeletePharmacyCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
