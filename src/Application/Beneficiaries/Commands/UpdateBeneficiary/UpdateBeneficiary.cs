using ApiBaseTemplate.Application.Common.Interfaces;

namespace ApiBaseTemplate.Application.Beneficiaries.Commands.UpdateBeneficiary;

public record UpdateBeneficiaryCommand : IRequest<int>
{
}

public class UpdateBeneficiaryCommandValidator : AbstractValidator<UpdateBeneficiaryCommand>
{
    public UpdateBeneficiaryCommandValidator()
    {
    }
}

public class UpdateBeneficiaryCommandHandler : IRequestHandler<UpdateBeneficiaryCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateBeneficiaryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> Handle(UpdateBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
