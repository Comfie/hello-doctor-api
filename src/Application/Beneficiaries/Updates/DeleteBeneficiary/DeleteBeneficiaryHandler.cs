using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Beneficiaries.Updates.DeleteBeneficiary;

public class DeleteBeneficiaryCommandHandler : IRequestHandler<DeleteBeneficiaryCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public DeleteBeneficiaryCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<Result<bool>> Handle(DeleteBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        var beneficiary = await _context.Beneficiaries.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        if (beneficiary is null) return Result<bool>.NotFound();
        if (string.IsNullOrWhiteSpace(_user.Id) || beneficiary.MainMemberId != _user.Id) return Result<bool>.Forbidden();

        beneficiary.IsDeleted = true;
        beneficiary.DeletedAt = DateTimeOffset.UtcNow;
        _context.Beneficiaries.Update(beneficiary);
        await _context.SaveChangesAsync(cancellationToken);

        _context.AuditLogs.Add(new AuditLog
        {
            Action = "BeneficiaryDeleted",
            ActorUserId = _user.Id,
            Details = $"BeneficiaryId={request.Id}"
        });
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}