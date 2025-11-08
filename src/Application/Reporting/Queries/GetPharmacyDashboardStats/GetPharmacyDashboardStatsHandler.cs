using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Reporting.Models;
using HelloDoctorApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Reporting.Queries.GetPharmacyDashboardStats;

public class GetPharmacyDashboardStatsHandler : IRequestHandler<GetPharmacyDashboardStatsQuery, Result<PharmacyDashboardStatsResponse>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeService _dateTime;
    private readonly IUser _user;

    public GetPharmacyDashboardStatsHandler(IApplicationDbContext db, IDateTimeService dateTime, IUser user)
    {
        _db = db;
        _dateTime = dateTime;
        _user = user;
    }

    public async Task<Result<PharmacyDashboardStatsResponse>> Handle(GetPharmacyDashboardStatsQuery request, CancellationToken ct)
    {
        // Validate pharmacy access for SystemAdministrators and Pharmacists
        var userPharmacyId = _user.GetPharmacyId();

        // If user has a pharmacyId (SystemAdmin or Pharmacist), they can only access their own pharmacy
        if (userPharmacyId.HasValue && userPharmacyId.Value != request.PharmacyId)
        {
            return Result<PharmacyDashboardStatsResponse>.Forbidden();
        }

        var now = _dateTime.OffsetNow;
        var todayStart = now.Date;
        var weekStart = now.AddDays(-7);
        var monthStart = now.AddMonths(-1);

        // Get all prescriptions for this pharmacy
        var allPrescriptions = await _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.AssignedPharmacyId == request.PharmacyId)
            .Select(p => new
            {
                p.Id,
                p.Status,
                p.IssuedDate,
                p.ExpiryDate,
                p.MainMemberId,
                p.BeneficiaryId,
                p.Created
            })
            .ToListAsync(ct);

        // Calculate metrics
        var totalPrescriptions = allPrescriptions.Count;
        var todaysPrescriptions = allPrescriptions.Count(p => p.Created >= todayStart);
        var weekPrescriptions = allPrescriptions.Count(p => p.Created >= weekStart);
        var monthPrescriptions = allPrescriptions.Count(p => p.Created >= monthStart);

        // Status counts
        var statusGroups = allPrescriptions
            .GroupBy(p => p.Status)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        var pendingReview = statusGroups.GetValueOrDefault(PrescriptionStatus.Pending.ToString(), 0);
        var underReview = statusGroups.GetValueOrDefault(PrescriptionStatus.UnderReview.ToString(), 0);
        var onHold = statusGroups.GetValueOrDefault(PrescriptionStatus.OnHold.ToString(), 0);
        var readyForDispensing = statusGroups.GetValueOrDefault(PrescriptionStatus.Approved.ToString(), 0);
        var partiallyDispensed = statusGroups.GetValueOrDefault(PrescriptionStatus.PartiallyDispensed.ToString(), 0);
        var fullyDispensed = statusGroups.GetValueOrDefault(PrescriptionStatus.FullyDispensed.ToString(), 0);
        var readyForPickup = statusGroups.GetValueOrDefault(PrescriptionStatus.ReadyForPickup.ToString(), 0);
        var delivered = statusGroups.GetValueOrDefault(PrescriptionStatus.Delivered.ToString(), 0);

        // Urgent attention
        var requiresAttention = pendingReview + onHold;
        var overdue = allPrescriptions.Count(p =>
            p.ExpiryDate < now &&
            p.Status != PrescriptionStatus.Delivered &&
            p.Status != PrescriptionStatus.FullyDispensed);

        // Completed metrics
        var completedStates = new[] { PrescriptionStatus.FullyDispensed, PrescriptionStatus.Delivered };
        var completedToday = allPrescriptions.Count(p =>
            completedStates.Contains(p.Status) && p.Created >= todayStart);
        var completedWeek = allPrescriptions.Count(p =>
            completedStates.Contains(p.Status) && p.Created >= weekStart);
        var completedMonth = allPrescriptions.Count(p =>
            completedStates.Contains(p.Status) && p.Created >= monthStart);

        // Performance - get average turnaround time
        var completedPrescriptions = await _db.PrescriptionStatusHistories
            .AsNoTracking()
            .Where(h => h.Prescription.AssignedPharmacyId == request.PharmacyId
                && (h.NewStatus == PrescriptionStatus.FullyDispensed || h.NewStatus == PrescriptionStatus.Delivered))
            .Select(h => new
            {
                h.PrescriptionId,
                h.ChangedDate,
                AssignedDate = h.Prescription.Created
            })
            .ToListAsync(ct);

        var avgTurnaround = completedPrescriptions.Any()
            ? completedPrescriptions.Average(p => (p.ChangedDate - p.AssignedDate).TotalHours)
            : 0;

        // Unique members and beneficiaries
        var totalMainMembers = allPrescriptions.Select(p => p.MainMemberId).Distinct().Count();
        var totalBeneficiaries = allPrescriptions.Select(p => p.BeneficiaryId).Distinct().Count();

        var response = new PharmacyDashboardStatsResponse
        {
            TotalPrescriptions = totalPrescriptions,
            TodaysPrescriptions = todaysPrescriptions,
            ThisWeekPrescriptions = weekPrescriptions,
            ThisMonthPrescriptions = monthPrescriptions,
            PendingReview = pendingReview,
            UnderReview = underReview,
            OnHold = onHold,
            ReadyForDispensing = readyForDispensing,
            PartiallyDispensed = partiallyDispensed,
            FullyDispensed = fullyDispensed,
            ReadyForPickup = readyForPickup,
            Delivered = delivered,
            PrescriptionsByStatus = statusGroups,
            RequiringAttention = requiresAttention,
            OverduePrescriptions = overdue,
            AverageTurnaroundHours = Math.Round(avgTurnaround, 2),
            CompletedToday = completedToday,
            CompletedThisWeek = completedWeek,
            CompletedThisMonth = completedMonth,
            TotalMainMembers = totalMainMembers,
            TotalBeneficiaries = totalBeneficiaries
        };

        return Result.Success(response);
    }
}
