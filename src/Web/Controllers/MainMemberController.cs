using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using HelloDoctorApi.Application.MainMembers.Commands.UpdateMyProfile;
using HelloDoctorApi.Application.MainMembers.Models;
using HelloDoctorApi.Application.MainMembers.Queries.GetMyProfile;
using HelloDoctorApi.Application.Prescriptions.Queries.GetMyPrescriptions;
using HelloDoctorApi.Application.Reporting.Queries.GetMyPrescriptionCounts;
using HelloDoctorApi.Application.Reporting.Queries.GetMyRecentActivity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoctorApi.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[TranslateResultToActionResult]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "MainMember")]
public class MainMemberController : ApiController
{
    public MainMemberController(ISender sender) : base(sender) { }

    [HttpGet("me/prescriptions")]
    public async Task<Result<List<MyPrescriptionItem>>> GetMyPrescriptions([FromQuery] string? beneficiaryCode, CancellationToken ct)
        => await Sender.Send(new GetMyPrescriptionsQuery(beneficiaryCode), ct);

    [HttpGet("me/reports/counts")]
    public async Task<Result<Dictionary<string,int>>> GetMyCounts(CancellationToken ct)
        => await Sender.Send(new GetMyPrescriptionCountsQuery(), ct);

    [HttpGet("me/reports/recent-activity")]
    public async Task<Result<List<MyRecentActivityItem>>> GetMyRecent([FromQuery] int limit, CancellationToken ct)
        => await Sender.Send(new GetMyRecentActivityQuery(limit <= 0 ? 20 : limit), ct);

    [HttpPut("me/preferences")]
    public async Task<Result<bool>> UpdatePreferences([FromBody] long? defaultPharmacyId, CancellationToken ct)
        => await Sender.Send(new HelloDoctorApi.Application.MainMembers.Commands.UpdatePreferences.UpdateMainMemberPreferencesCommand(defaultPharmacyId), ct);

    [HttpGet("me/profile")]
    public async Task<Result<MainMemberProfileResponse>> GetMyProfile(CancellationToken ct)
        => await Sender.Send(new GetMyProfileQuery(), ct);

    [HttpPut("me/profile")]
    public async Task<Result<MainMemberProfileResponse>> UpdateMyProfile([FromBody] UpdateMyProfileCommand command, CancellationToken ct)
        => await Sender.Send(command, ct);
}
