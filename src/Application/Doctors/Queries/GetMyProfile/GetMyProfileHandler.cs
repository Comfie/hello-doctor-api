using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Doctors.Models;
using HelloDoctorApi.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Doctors.Queries.GetMyProfile;

public class GetMyProfileHandler : IRequestHandler<GetMyProfileQuery, Result<DoctorResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetMyProfileHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<Result<DoctorResponse>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id;

        if (string.IsNullOrWhiteSpace(userId))
            return Result<DoctorResponse>.Unauthorized();

        // Find doctor by AccountId (linked to ApplicationUser)
        var doctor = await _context.Doctors
            .Include(d => d.Pharmacies)
            .FirstOrDefaultAsync(d => d.AccountId == userId, cancellationToken);

        if (doctor == null)
            return Result<DoctorResponse>.NotFound();

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
