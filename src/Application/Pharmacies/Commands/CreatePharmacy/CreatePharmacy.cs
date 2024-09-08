using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Pharmacies.Models;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;

namespace HelloDoctorApi.Application.Pharmacies.Commands.CreatePharmacy;

public record CreatePharmacyCommand(CreatePharmacyRequest Request) : IRequest<Result<PharmacyResponse>>;

public class CreatePharmacyCommandValidator : AbstractValidator<CreatePharmacyCommand>
{
    public CreatePharmacyCommandValidator()
    {
    }
}

public class CreatePharmacyCommandHandler : IRequestHandler<CreatePharmacyCommand, Result<PharmacyResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePharmacyCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PharmacyResponse>> Handle(CreatePharmacyCommand request,
        CancellationToken cancellationToken)
    {

        var pharmacy = new Pharmacy
        {
            Name = request.Request.Name,
            Description = request.Request.Description,
            ContactNumber = request.Request.ContactNumber,
            ContactEmail = request.Request.ContactEmail,
            ContactPerson = request.Request.ContactPerson,
            Address = request.Request.Address,
            OpeningTime = request.Request.OpeningTime,
            ClosingTime = request.Request.ClosingTime,
            IsActive = false
        };

        _context.Pharmacies.Add(pharmacy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new PharmacyResponse
        {
            Id = pharmacy.Id,
            Name = pharmacy.Name,
            Description = pharmacy.Description,
            ContactNumber = pharmacy.ContactNumber,
            ContactEmail = pharmacy.ContactEmail,
            ContactPerson = pharmacy.ContactPerson,
            Address = pharmacy.Address,
            OpeningTime = pharmacy.OpeningTime,
            ClosingTime = pharmacy.ClosingTime,
            IsActive = pharmacy.IsActive
        });
    }
}