using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Reporting.Models;
using HelloDoctorApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Application.Reporting.Queries.GetPharmacyPerformanceMetrics;

public class GetPharmacyPerformanceMetricsHandler : IRequestHandler<GetPharmacyPerformanceMetricsQuery, Result<PerformanceMetricsResponse>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeService _dateTime;
    private readonly IUser _user;

    public GetPharmacyPerformanceMetricsHandler(IApplicationDbContext db, IDateTimeService dateTime, IUser user)
    {
        _db = db;
        _dateTime = dateTime;
        _user = user;
    }

    public async Task<Result<PerformanceMetricsResponse>> Handle(GetPharmacyPerformanceMetricsQuery request, CancellationToken ct)
    {
        // Validate pharmacy access
        var userPharmacyId = _user.GetPharmacyId();
        if (userPharmacyId.HasValue && userPharmacyId.Value != request.PharmacyId)
        {
            return Result<PerformanceMetricsResponse>.Forbidden();
        }

        var now = _dateTime.OffsetNow;
        var startDate = now.AddDays(-request.DaysBack);

        // Get completed prescriptions with their turnaround times
        var completedPrescriptions = await _db.Prescriptions
            .AsNoTracking()
            .Where(p => p.AssignedPharmacyId == request.PharmacyId
                && p.Created >= startDate
                && (p.Status == PrescriptionStatus.FullyDispensed || p.Status == PrescriptionStatus.Delivered))
            .Select(p => new
            {
                p.Id,
                p.Created,
                CompletedDate = p.StatusHistory!
                    .Where(h => h.NewStatus == PrescriptionStatus.FullyDispensed || h.NewStatus == PrescriptionStatus.Delivered)
                    .OrderByDescending(h => h.ChangedDate)
                    .Select(h => h.ChangedDate)
                    .FirstOrDefault()
            })
            .ToListAsync(ct);

        var turnaroundHours = completedPrescriptions
            .Where(p => p.CompletedDate != default)
            .Select(p => (p.CompletedDate - p.Created).TotalHours)
            .OrderBy(h => h)
            .ToList();

        // Calculate statistics
        var avgTurnaround = turnaroundHours.Any() ? turnaroundHours.Average() : 0;
        var medianTurnaround = turnaroundHours.Any()
            ? turnaroundHours[turnaroundHours.Count / 2]
            : 0;
        var fastestTurnaround = turnaroundHours.Any() ? turnaroundHours.First() : 0;
        var slowestTurnaround = turnaroundHours.Any() ? turnaroundHours.Last() : 0;

        // Get total assigned and completion rate
        var totalAssigned = await _db.Prescriptions
            .AsNoTracking()
            .CountAsync(p => p.AssignedPharmacyId == request.PharmacyId && p.Created >= startDate, ct);

        var totalCompleted = completedPrescriptions.Count;
        var completionRate = totalAssigned > 0 ? (totalCompleted / (double)totalAssigned) * 100 : 0;

        // Get daily metrics for the last 7 days
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => now.AddDays(-i).Date)
            .OrderBy(d => d)
            .ToList();

        var dailyMetrics = new List<DailyMetric>();
        foreach (var day in last7Days)
        {
            var dayStart = day;
            var dayEnd = day.AddDays(1);

            var received = await _db.Prescriptions
                .AsNoTracking()
                .CountAsync(p => p.AssignedPharmacyId == request.PharmacyId
                    && p.Created >= dayStart && p.Created < dayEnd, ct);

            var completed = completedPrescriptions
                .Count(p => p.CompletedDate >= dayStart && p.CompletedDate < dayEnd);

            var dayTurnaround = completedPrescriptions
                .Where(p => p.CompletedDate >= dayStart && p.CompletedDate < dayEnd && p.CompletedDate != default)
                .Select(p => (p.CompletedDate - p.Created).TotalHours)
                .DefaultIfEmpty(0)
                .Average();

            dailyMetrics.Add(new DailyMetric(
                DateOnly.FromDateTime(day),
                received,
                completed,
                Math.Round(dayTurnaround, 2)
            ));
        }

        // Get current in-progress and on-hold counts
        var currentInProgress = await _db.Prescriptions
            .AsNoTracking()
            .CountAsync(p => p.AssignedPharmacyId == request.PharmacyId
                && p.Status == PrescriptionStatus.UnderReview, ct);

        var currentOnHold = await _db.Prescriptions
            .AsNoTracking()
            .CountAsync(p => p.AssignedPharmacyId == request.PharmacyId
                && p.Status == PrescriptionStatus.OnHold, ct);

        var response = new PerformanceMetricsResponse
        {
            AverageTurnaroundHours = Math.Round(avgTurnaround, 2),
            MedianTurnaroundHours = Math.Round(medianTurnaround, 2),
            FastestTurnaroundHours = Math.Round(fastestTurnaround, 2),
            SlowestTurnaroundHours = Math.Round(slowestTurnaround, 2),
            TotalAssigned = totalAssigned,
            TotalCompleted = totalCompleted,
            CompletionRate = Math.Round(completionRate, 2),
            DailyMetrics = dailyMetrics,
            CurrentlyInProgress = currentInProgress,
            CurrentlyOnHold = currentOnHold
        };

        return Result.Success(response);
    }
}
