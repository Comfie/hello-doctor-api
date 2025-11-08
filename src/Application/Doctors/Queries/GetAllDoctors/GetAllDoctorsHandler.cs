using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Doctors.Models;
using HelloDoctorApi.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Doctors.Queries.GetAllDoctors;

public class GetAllDoctorsHandler : IRequestHandler<GetAllDoctorsQuery, Result<List<DoctorResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllDoctorsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<DoctorResponse>>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
    {
        var doctors = await _context.Doctors
            .Include(d => d.Pharmacies)
            .Select(d => new DoctorResponse(
                d.Id,
                d.FirstName,
                d.LastName,
                d.EmailAddress,
                d.PrimaryContact,
                d.SecondaryContact,
                d.QualificationDescription,
                d.Speciality,
                d.IsActive,
                d.Pharmacies != null ? d.Pharmacies.Select(p => p.Name).ToList() : null
            ))
            .ToListAsync(cancellationToken);

        return Result.Success(doctors);
    }
}
