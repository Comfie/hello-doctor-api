using Ardalis.Result;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Enums;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Updates.UpdateBeneficiary;

public class UpdateBeneficiaryHandler : IRequestHandler<UpdateBeneficiaryCommand, Result<BeneficiaryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUser _user;

    public UpdateBeneficiaryHandler(IApplicationDbContext context,
        IUnitOfWork unitOfWork, IUser user)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _user = user;
    }

    public async Task<Result<BeneficiaryResponse>> Handle(UpdateBeneficiaryCommand request,
        CancellationToken cancellationToken)
    {
        var beneficiary = await _context
            .Beneficiaries
            .Include(beneficiary => beneficiary.MainMember)
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (beneficiary is null)
        {
            return Result<BeneficiaryResponse>.NotFound(new Error("Beneficiary", "Beneficiary not found"));
        }

        // Get MainMemberId from JWT claims (returns long?)
        var userMainMemberId = _user.GetMainMemberId();

        // Validate ownership: beneficiary must belong to the requesting MainMember
        if (!userMainMemberId.HasValue || beneficiary.MainMemberId != userMainMemberId.Value)
        {
            return Result<BeneficiaryResponse>.Forbidden();
        }

        beneficiary.FirstName = request.FirstName ?? beneficiary.FirstName;
        beneficiary.LastName = request.LastName ?? beneficiary.LastName;
        beneficiary.PhoneNumber = request.PhoneNumber ?? beneficiary.PhoneNumber;
        beneficiary.EmailAddress = request.EmailAddress ?? beneficiary.EmailAddress;
        beneficiary.Gender = request.Gender ?? beneficiary.Gender;
        beneficiary.DateOfBirth = request.DateOfBirth ?? beneficiary.DateOfBirth;
        beneficiary.Relationship = request.RelationshipToMainMember switch
        {
            "sister" => RelationshipToMainMember.Sister,
            "brother" => RelationshipToMainMember.Brother,
            "parent" => RelationshipToMainMember.Parent,
            "child" => RelationshipToMainMember.Child,
            _ => RelationshipToMainMember.Other
        };
        _context.Beneficiaries.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _context.AuditLogs.Add(new AuditLog
        {
            Action = "BeneficiaryUpdated",
            ActorUserId = _user.Id,
            Details = $"BeneficiaryId={beneficiary.Id}"
        });
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new BeneficiaryResponse(
            beneficiary.Id,
            beneficiary.MainMember.FirstName + " " + beneficiary.MainMember.LastName,
            beneficiary.MainMember.PhoneNumber ?? string.Empty,
            beneficiary.FirstName,
            beneficiary.LastName,
            beneficiary.PhoneNumber,
            beneficiary.EmailAddress,
            beneficiary.MainMemberId,
            beneficiary.Relationship.ToString(),
            beneficiary.Gender,
            beneficiary.DateOfBirth,
            beneficiary.BeneficiaryCode
        ));
    }
}