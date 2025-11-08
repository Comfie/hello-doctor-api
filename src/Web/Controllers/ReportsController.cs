using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using HelloDoctorApi.Application.Reporting.Queries.GetPharmacyPrescriptionCounts;
using HelloDoctorApi.Application.Reporting.Queries.GetRecentPrescriptionActivity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoctorApi.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdministrator,SystemAdministrator,Pharmacist")]
[TranslateResultToActionResult]
public class ReportsController : ApiController
{
    public ReportsController(ISender sender) : base(sender) {}

    [HttpGet("pharmacies/{pharmacyId:long}/counts")]
    public async Task<Result<Dictionary<string,int>>> GetCounts(long pharmacyId, CancellationToken ct)
        => await Sender.Send(new GetPharmacyPrescriptionCountsQuery(pharmacyId), ct);

    [HttpGet("pharmacies/{pharmacyId:long}/recent-activity")]
    public async Task<Result<List<RecentActivityItem>>> GetRecentActivity(long pharmacyId, [FromQuery] int limit, CancellationToken ct)
        => await Sender.Send(new GetRecentPrescriptionActivityQuery(pharmacyId, limit <= 0 ? 20 : limit), ct);
}

