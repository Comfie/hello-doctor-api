using ApiBaseTemplate.Application.Beneficiaries.Models;
using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Domain.Entities;
using ApiBaseTemplate.Domain.Enums;
using ApiBaseTemplate.Domain.Repositories;
using ApiBaseTemplate.Domain.Shared;

namespace ApiBaseTemplate.Application.Beneficiaries.Commands.CreateBeneficiary;

public record CreateBeneficiaryCommand(CreateBeneficiaryRequest Request) : IRequest<Result<long>>;

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
