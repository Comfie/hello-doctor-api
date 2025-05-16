using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Updates.DeleteBeneficiary;

public class DeleteBeneficiaryCommandHandler : IRequestHandler<DeleteBeneficiaryCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteBeneficiaryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        await _context
            .Beneficiaries
            .Where(x => x.Id == request.Id)
            .ExecuteUpdateAsync(calls => calls
                    .SetProperty(x => x.IsDeleted, true)
                    .SetProperty(x => x.LastModified, DateTime.Now)
                , cancellationToken);

        return Result.Success(true);
    }
}