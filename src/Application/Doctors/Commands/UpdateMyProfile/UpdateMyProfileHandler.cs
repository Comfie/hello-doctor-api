using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Doctors.Models;
using HelloDoctorApi.Domain.Repositories;
using HelloDoctorApi.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Doctors.Commands.UpdateMyProfile;

public class UpdateMyProfileHandler : IRequestHandler<UpdateMyProfileCommand, Result<DoctorResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUser _user;

    public UpdateMyProfileHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IUser user)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _user = user;
    }

    public async Task<Result<DoctorResponse>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;

        if (string.IsNullOrWhiteSpace(userId))
            return Result<DoctorResponse>.Unauthorized();

        // Find doctor by linked ApplicationUser account
        var doctor = await _context.Doctors
            .Include(d => d.Pharmacies)
            .FirstOrDefaultAsync(d => d.EmailAddress == userId, cancellationToken);

        if (doctor == null)
            return Result<DoctorResponse>.NotFound();

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(request.Request.FirstName))
            doctor.FirstName = request.Request.FirstName;

        if (!string.IsNullOrWhiteSpace(request.Request.LastName))
            doctor.LastName = request.Request.LastName;

        if (!string.IsNullOrWhiteSpace(request.Request.EmailAddress))
            doctor.EmailAddress = request.Request.EmailAddress;

        if (!string.IsNullOrWhiteSpace(request.Request.PrimaryContact))
            doctor.PrimaryContact = request.Request.PrimaryContact;

        if (request.Request.SecondaryContact != null)
            doctor.SecondaryContact = request.Request.SecondaryContact;

        if (request.Request.QualificationDescription != null)
            doctor.QualificationDescription = request.Request.QualificationDescription;

        if (request.Request.Speciality != null)
            doctor.Speciality = request.Request.Speciality;

        _context.Doctors.Update(doctor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new DoctorResponse(
            doctor.Id,
            doctor.FirstName,
            doctor.LastName,
            doctor.EmailAddress,
            doctor.PrimaryContact,
            doctor.SecondaryContact,
            doctor.QualificationDescription,
            doctor.Speciality,
            doctor.IsActive,
            doctor.Pharmacies?.Select(p => p.Name).ToList()
        );

        return Result.Success(response);
    }
}
