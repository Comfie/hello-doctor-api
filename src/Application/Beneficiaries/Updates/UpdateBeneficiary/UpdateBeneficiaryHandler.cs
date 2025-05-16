using Ardalis.Result;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Beneficiaries.Updates.UpdateBeneficiary;

public class UpdateBeneficiaryHandler : IRequestHandler<UpdateBeneficiaryCommand, Result<BeneficiaryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBeneficiaryHandler(IApplicationDbContext context,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
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

        beneficiary.FirstName = request.FirstName ?? beneficiary.FirstName;
        beneficiary.LastName = request.LastName ?? beneficiary.LastName;
        beneficiary.PhoneNumber = request.PhoneNumber ?? beneficiary.PhoneNumber;
        beneficiary.EmailAddress = request.EmailAddress ?? beneficiary.EmailAddress;
        _context.Beneficiaries.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new BeneficiaryResponse(
            beneficiary.Id,
            beneficiary.MainMember.FirstName + " " + beneficiary.MainMember.LastName,
            beneficiary.MainMember.PhoneNumber ?? string.Empty,
            beneficiary.FirstName,
            beneficiary.LastName,
            beneficiary.PhoneNumber,
            beneficiary.EmailAddress,
            beneficiary.MainMemberId,
            beneficiary.Relationship.ToString()
        ));
    }
}