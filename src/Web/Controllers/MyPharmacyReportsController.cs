using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Reporting.Models;
using HelloDoctorApi.Application.Reporting.Queries.GetOverduePrescriptions;
using HelloDoctorApi.Application.Reporting.Queries.GetPharmacyDashboardStats;
using HelloDoctorApi.Application.Reporting.Queries.GetPharmacyPerformanceMetrics;
using HelloDoctorApi.Application.Reporting.Queries.GetPrescriptionsByDateRange;
using HelloDoctorApi.Application.Reporting.Queries.GetPrescriptionsByStatus;
using HelloDoctorApi.Application.Reporting.Queries.GetTodaysPrescriptions;
using HelloDoctorApi.Application.Reporting.Queries.GetTopMembers;
using HelloDoctorApi.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginatedResponse = HelloDoctorApi.Application.Reporting.Models.PaginatedPrescriptionsResponse;

namespace HelloDoctorApi.Web.Controllers;

/// <summary>
/// My pharmacy reports - automatically uses the logged-in pharmacist's pharmacy
/// </summary>
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/my-pharmacy/reports")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist")] 
[TranslateResultToActionResult]
public class MyPharmacyReportsController : ApiController
{
    private readonly IUser _user;

    public MyPharmacyReportsController(ISender sender, IUser user) : base(sender)
    {
        _user = user;
    }

    private long GetPharmacyId()
    {
        var pharmacyIdClaim = _user.GetPharmacyId();
        if (!pharmacyIdClaim.HasValue)
            throw new UnauthorizedAccessException("Pharmacist must be associated with a pharmacy");
        return pharmacyIdClaim.Value;
    }

    /// <summary>
    /// Get my pharmacy's dashboard statistics
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(PharmacyDashboardStatsResponse), StatusCodes.Status200OK)]
    public async Task<Result<PharmacyDashboardStatsResponse>> GetMyDashboardStats(CancellationToken ct)
        => await Sender.Send(new GetPharmacyDashboardStatsQuery(GetPharmacyId()), ct);

    /// <summary>
    /// Get today's prescriptions for my pharmacy
    /// </summary>
    [HttpGet("todays-prescriptions")]
    [ProducesResponseType(typeof(List<PrescriptionListItem>), StatusCodes.Status200OK)]
    public async Task<Result<List<PrescriptionListItem>>> GetTodaysPrescriptions(CancellationToken ct)
        => await Sender.Send(new GetTodaysPrescriptionsQuery(GetPharmacyId()), ct);

    /// <summary>
    /// Get my pharmacy's overdue prescriptions
    /// </summary>
    [HttpGet("overdue")]
    [ProducesResponseType(typeof(List<PrescriptionListItem>), StatusCodes.Status200OK)]
    public async Task<Result<List<PrescriptionListItem>>> GetOverduePrescriptions(CancellationToken ct)
        => await Sender.Send(new GetOverduePrescriptionsQuery(GetPharmacyId()), ct);

    /// <summary>
    /// Get my pharmacy's prescriptions by status
    /// </summary>
    [HttpGet("by-status")]
    [ProducesResponseType(typeof(PaginatedResponse), StatusCodes.Status200OK)]
    public async Task<Result<PaginatedResponse>> GetPrescriptionsByStatus(
        [FromQuery] PrescriptionStatus status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        pageSize = Math.Min(pageSize, 100);
        return await Sender.Send(new GetPrescriptionsByStatusQuery(GetPharmacyId(), status, pageNumber, pageSize), ct);
    }

    /// <summary>
    /// Get my pharmacy's prescriptions by date range
    /// </summary>
    [HttpGet("by-date-range")]
    [ProducesResponseType(typeof(PaginatedResponse), StatusCodes.Status200OK)]
    public async Task<Result<PaginatedResponse>> GetPrescriptionsByDateRange(
        [FromQuery] DateTimeOffset startDate,
        [FromQuery] DateTimeOffset endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        pageSize = Math.Min(pageSize, 100);
        return await Sender.Send(new GetPrescriptionsByDateRangeQuery(GetPharmacyId(), startDate, endDate, pageNumber, pageSize), ct);
    }

    /// <summary>
    /// Get my pharmacy's performance metrics
    /// </summary>
    [HttpGet("performance")]
    [ProducesResponseType(typeof(PerformanceMetricsResponse), StatusCodes.Status200OK)]
    public async Task<Result<PerformanceMetricsResponse>> GetPerformanceMetrics(
        [FromQuery] int daysBack = 30,
        CancellationToken ct = default)
    {
        daysBack = Math.Min(daysBack, 90);
        return await Sender.Send(new GetPharmacyPerformanceMetricsQuery(GetPharmacyId(), daysBack), ct);
    }

    /// <summary>
    /// Get my pharmacy's top members
    /// </summary>
    [HttpGet("top-members")]
    [ProducesResponseType(typeof(List<TopMemberResponse>), StatusCodes.Status200OK)]
    public async Task<Result<List<TopMemberResponse>>> GetTopMembers(
        [FromQuery] int topCount = 10,
        [FromQuery] int daysBack = 30,
        CancellationToken ct = default)
    {
        topCount = Math.Min(topCount, 50);
        daysBack = Math.Min(daysBack, 90);
        return await Sender.Send(new GetTopMembersQuery(GetPharmacyId(), topCount, daysBack), ct);
    }
}
