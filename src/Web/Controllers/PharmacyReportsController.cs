using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using HelloDoctorApi.Application.Reporting.Models;
using HelloDoctorApi.Application.Reporting.Queries.GetOverduePrescriptions;
using HelloDoctorApi.Application.Reporting.Queries.GetPharmacyDashboardStats;
using HelloDoctorApi.Application.Reporting.Queries.GetPharmacyPerformanceMetrics;
using HelloDoctorApi.Application.Reporting.Queries.GetPrescriptionsByDateRange;
using HelloDoctorApi.Application.Reporting.Queries.GetPrescriptionsByStatus;
using HelloDoctorApi.Application.Reporting.Queries.GetTodaysPrescriptions;
using HelloDoctorApi.Application.Reporting.Queries.GetTopMembers;
using HelloDoctorApi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginatedResponse = HelloDoctorApi.Application.Reporting.Models.PaginatedPrescriptionsResponse;

namespace HelloDoctorApi.Web.Controllers;

/// <summary>
/// Pharmacy-specific reporting endpoints for pharmacists
/// </summary>
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/pharmacy-reports")]
[Authorize(Roles = "Pharmacist,SystemAdministrator,SuperAdministrator")]
[TranslateResultToActionResult]
public class PharmacyReportsController : ApiController
{
    public PharmacyReportsController(ISender sender) : base(sender) { }

    /// <summary>
    /// Get comprehensive dashboard statistics for the pharmacy
    /// </summary>
    /// <param name="pharmacyId">Pharmacy identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Dashboard statistics including counts, trends, and metrics</returns>
    [HttpGet("{pharmacyId:long}/dashboard")]
    [ProducesResponseType(typeof(PharmacyDashboardStatsResponse), StatusCodes.Status200OK)]
    public async Task<Result<PharmacyDashboardStatsResponse>> GetDashboardStats(
        long pharmacyId,
        CancellationToken ct)
        => await Sender.Send(new GetPharmacyDashboardStatsQuery(pharmacyId), ct);

    /// <summary>
    /// Get today's prescriptions for the pharmacy
    /// </summary>
    /// <param name="pharmacyId">Pharmacy identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of prescriptions received today</returns>
    [HttpGet("{pharmacyId:long}/todays-prescriptions")]
    [ProducesResponseType(typeof(List<PrescriptionListItem>), StatusCodes.Status200OK)]
    public async Task<Result<List<PrescriptionListItem>>> GetTodaysPrescriptions(
        long pharmacyId,
        CancellationToken ct)
        => await Sender.Send(new GetTodaysPrescriptionsQuery(pharmacyId), ct);

    /// <summary>
    /// Get overdue prescriptions (expired but not yet dispensed)
    /// </summary>
    /// <param name="pharmacyId">Pharmacy identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of overdue prescriptions requiring attention</returns>
    [HttpGet("{pharmacyId:long}/overdue")]
    [ProducesResponseType(typeof(List<PrescriptionListItem>), StatusCodes.Status200OK)]
    public async Task<Result<List<PrescriptionListItem>>> GetOverduePrescriptions(
        long pharmacyId,
        CancellationToken ct)
        => await Sender.Send(new GetOverduePrescriptionsQuery(pharmacyId), ct);

    /// <summary>
    /// Get prescriptions filtered by status with pagination
    /// </summary>
    /// <param name="pharmacyId">Pharmacy identifier</param>
    /// <param name="status">Prescription status to filter by</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50, max: 100)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of prescriptions with the specified status</returns>
    [HttpGet("{pharmacyId:long}/by-status")]
    [ProducesResponseType(typeof(PaginatedResponse), StatusCodes.Status200OK)]
    public async Task<Result<PaginatedResponse>> GetPrescriptionsByStatus(
        long pharmacyId,
        [FromQuery] PrescriptionStatus status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        // Limit page size to prevent abuse
        pageSize = Math.Min(pageSize, 100);
        return await Sender.Send(new GetPrescriptionsByStatusQuery(pharmacyId, status, pageNumber, pageSize), ct);
    }

    /// <summary>
    /// Get prescriptions within a date range with pagination
    /// </summary>
    /// <param name="pharmacyId">Pharmacy identifier</param>
    /// <param name="startDate">Start date (inclusive)</param>
    /// <param name="endDate">End date (inclusive)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50, max: 100)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of prescriptions within the date range</returns>
    [HttpGet("{pharmacyId:long}/by-date-range")]
    [ProducesResponseType(typeof(PaginatedResponse), StatusCodes.Status200OK)]
    public async Task<Result<PaginatedResponse>> GetPrescriptionsByDateRange(
        long pharmacyId,
        [FromQuery] DateTimeOffset startDate,
        [FromQuery] DateTimeOffset endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        pageSize = Math.Min(pageSize, 100);
        return await Sender.Send(new GetPrescriptionsByDateRangeQuery(pharmacyId, startDate, endDate, pageNumber, pageSize), ct);
    }

    /// <summary>
    /// Get performance metrics for the pharmacy
    /// </summary>
    /// <param name="pharmacyId">Pharmacy identifier</param>
    /// <param name="daysBack">Number of days to look back (default: 30)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Performance metrics including turnaround times and completion rates</returns>
    [HttpGet("{pharmacyId:long}/performance")]
    [ProducesResponseType(typeof(PerformanceMetricsResponse), StatusCodes.Status200OK)]
    public async Task<Result<PerformanceMetricsResponse>> GetPerformanceMetrics(
        long pharmacyId,
        [FromQuery] int daysBack = 30,
        CancellationToken ct = default)
    {
        // Limit to max 90 days to prevent performance issues
        daysBack = Math.Min(daysBack, 90);
        return await Sender.Send(new GetPharmacyPerformanceMetricsQuery(pharmacyId, daysBack), ct);
    }

    /// <summary>
    /// Get top members by prescription volume
    /// </summary>
    /// <param name="pharmacyId">Pharmacy identifier</param>
    /// <param name="topCount">Number of top members to return (default: 10)</param>
    /// <param name="daysBack">Number of days to look back (default: 30)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of top members with their prescription statistics</returns>
    [HttpGet("{pharmacyId:long}/top-members")]
    [ProducesResponseType(typeof(List<TopMemberResponse>), StatusCodes.Status200OK)]
    public async Task<Result<List<TopMemberResponse>>> GetTopMembers(
        long pharmacyId,
        [FromQuery] int topCount = 10,
        [FromQuery] int daysBack = 30,
        CancellationToken ct = default)
    {
        topCount = Math.Min(topCount, 50); // Limit to 50
        daysBack = Math.Min(daysBack, 90);
        return await Sender.Send(new GetTopMembersQuery(pharmacyId, topCount, daysBack), ct);
    }
}
