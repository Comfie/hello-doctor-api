using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Enums;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Commands.CreateBeneficiary;

public class CreateBeneficiaryCommandHandler : IRequestHandler<CreateBeneficiaryCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBeneficiaryCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<long>> Handle(CreateBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        var mainMember = await _context
            .ApplicationUsers
            .SingleAsync(x => x.Id == request.Request.MainMemberId, cancellationToken);

        var beneficiary = new Beneficiary
        {
            FirstName = request.Request.FirstName,
            LastName = request.Request.LastName,
            PhoneNumber = request.Request.PhoneNumber,
            EmailAddress = request.Request.EmailAddress,
            BeneficiaryCode = "To Be Determined",
            MainMemberId = mainMember.Id,
            MainMember = mainMember
        };

        var relationshipEnum = request.Request.Relationship switch
        {
            "sister" => RelationshipToMainMember.Sister,
            "brother" => RelationshipToMainMember.Brother,
            "parent" => RelationshipToMainMember.Parent,
            "child" => RelationshipToMainMember.Child,
            _ => RelationshipToMainMember.Other
        };

        beneficiary.Relationship = relationshipEnum;

        _context.Beneficiaries.Add(beneficiary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(beneficiary.Id);
    }
}