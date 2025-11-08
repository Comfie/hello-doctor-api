using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Reporting.Models;
using HelloDoctorApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Reporting.Queries.GetTopMembers;

public class GetTopMembersHandler : IRequestHandler<GetTopMembersQuery, Result<List<TopMemberResponse>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeService _dateTime;
    private readonly IUser _user;

    public GetTopMembersHandler(IApplicationDbContext db, IDateTimeService dateTime, IUser user)
    {
        _db = db;
        _dateTime = dateTime;
        _user = user;
    }

    public async Task<Result<List<TopMemberResponse>>> Handle(GetTopMembersQuery request, CancellationToken ct)
    {
        // Validate pharmacy access
        var userPharmacyId = _user.GetPharmacyId();
        if (userPharmacyId.HasValue && userPharmacyId.Value != request.PharmacyId)
        {
            return Result<List<TopMemberResponse>>.Forbidden();
        }

        var startDate = _dateTime.OffsetNow.AddDays(-request.DaysBack);

        var topMembers = await _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.AssignedPharmacyId == request.PharmacyId && p.Created >= startDate)
            .GroupBy(p => new
            {
                p.MainMemberId,
                MainMemberName = p.MainMember.FirstName + " " + p.MainMember.LastName,
                MemberCode = p.MainMember.MainMember!.Code
            })
            .Select(g => new
            {
                g.Key.MainMemberId,
                g.Key.MainMemberName,
                g.Key.MemberCode,
                TotalPrescriptions = g.Count(),
                PendingPrescriptions = g.Count(p => p.Status == PrescriptionStatus.Pending
                    || p.Status == PrescriptionStatus.UnderReview
                    || p.Status == PrescriptionStatus.OnHold),
                CompletedPrescriptions = g.Count(p => p.Status == PrescriptionStatus.Delivered
                    || p.Status == PrescriptionStatus.FullyDispensed),
                LastPrescriptionDate = g.Max(p => (DateTimeOffset?)p.Created),
                Email = g.Max(p => p.MainMember.Email),
                PhoneNumber = g.Max(p => p.MainMember.PhoneNumber)
            })
            .OrderByDescending(x => x.TotalPrescriptions)
            .Take(request.TopCount)
            .ToListAsync(ct);

        var response = topMembers.Select(m => new TopMemberResponse(
            m.MainMemberId,
            m.MainMemberName,
            m.MemberCode,
            m.TotalPrescriptions,
            m.PendingPrescriptions,
            m.CompletedPrescriptions,
            m.LastPrescriptionDate,
            m.Email ?? string.Empty,
            m.PhoneNumber ?? string.Empty
        )).ToList();

        return Result.Success(response);
    }
}
