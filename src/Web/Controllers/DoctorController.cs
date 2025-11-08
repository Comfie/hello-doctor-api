using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using HelloDoctorApi.Application.Doctors.Commands.CreatePrescription;
using HelloDoctorApi.Application.Doctors.Commands.UpdateMyProfile;
using HelloDoctorApi.Application.Doctors.Models;
using HelloDoctorApi.Application.Doctors.Queries.GetAllDoctors;
using HelloDoctorApi.Application.Doctors.Queries.GetMyProfile;
using HelloDoctorApi.Application.Doctors.Queries.GetMyPrescriptions;
using HelloDoctorApi.Application.MainMembers.Models;
using HelloDoctorApi.Application.MainMembers.Queries.GetMainMembers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoctorApi.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[TranslateResultToActionResult]
public class DoctorController : ApiController
{
    public DoctorController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    ///     Get my doctor profile
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Doctor profile details</returns>
    [HttpGet("my-profile")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<Result<DoctorResponse>> GetMyProfile(CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetMyProfileQuery(), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Update my doctor profile
    /// </summary>
    /// <param name="request">Updated profile information</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Updated doctor profile</returns>
    [HttpPut("my-profile")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<Result<DoctorResponse>> UpdateMyProfile([FromBody] UpdateDoctorProfileRequest request, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new UpdateMyProfileCommand(request), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Get all doctors (Admin only)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of all doctors</returns>
    [HttpGet("all")]
    [Authorize(Roles = "SuperAdministrator,SystemAdministrator")]
    [ProducesResponseType(typeof(List<DoctorResponse>), StatusCodes.Status200OK)]
    public async Task<Result<List<DoctorResponse>>> GetAllDoctors(CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetAllDoctorsQuery(), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Create a new prescription for a beneficiary
    /// </summary>
    /// <param name="request">Prescription details</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Created prescription ID</returns>
    [HttpPost("prescriptions")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<Result<long>> CreatePrescription([FromBody] CreatePrescriptionCommand request, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(request, cancellationToken);
        return response;
    }

    /// <summary>
    ///     Get my prescriptions (created by me)
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of prescriptions created by this doctor</returns>
    [HttpGet("prescriptions")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(List<DoctorPrescriptionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<Result<List<DoctorPrescriptionResponse>>> GetMyPrescriptions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var response = await Sender.Send(new GetMyPrescriptionsQuery(pageNumber, pageSize), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Get all main members (for prescription creation)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of all main members</returns>
    [HttpGet("main-members")]
    [Authorize(Roles = "Doctor,SuperAdministrator")]
    [ProducesResponseType(typeof(List<MainMemberResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<Result<List<MainMemberResponse>>> GetAllMainMembers(CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetMainMembersQuery(), cancellationToken);
        return response;
    }
}
