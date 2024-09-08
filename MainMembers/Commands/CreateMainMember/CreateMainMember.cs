using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.MainMembers.Commands.CreateMainMember;

public record CreateMainMemberCommand : IRequest<bool>
{
}

public class CreateMainMemberCommandValidator : AbstractValidator<CreateMainMemberCommand>
{
    public CreateMainMemberCommandValidator()
    {
    }
}

public class CreateMainMemberCommandHandler : IRequestHandler<CreateMainMemberCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CreateMainMemberCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CreateMainMemberCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
