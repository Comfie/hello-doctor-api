using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using HelloDoctorApi.Application.Pharmacies.Commands.CreatePharmacy;
using HelloDoctorApi.Application.Pharmacies.Commands.UploadPharmacyLogo;
using HelloDoctorApi.Application.Pharmacies.Models;
using HelloDoctorApi.Application.Pharmacies.Queries.GetActivePharmacies;
using HelloDoctorApi.Application.Pharmacies.Queries.GetAllPharmacies;
using HelloDoctorApi.Application.Pharmacies.Queries.GetByIdPharmacy;
using HelloDoctorApi.Application.Pharmacies.Queries.GetMyPharmacy;
using HelloDoctorApi.Application.Pharmacies.Updates.DeletePharmacy;
using HelloDoctorApi.Application.Pharmacies.Updates.TogglePharmacyActive;
using HelloDoctorApi.Application.Pharmacies.Updates.UpdatePharmacy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoctorApi.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[TranslateResultToActionResult]
public class PharmacyController : ApiController
{
    public PharmacyController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    ///     Creates a new pharmacy
    /// </summary>
    /// <param name="request">the details for the pharmacy</param>
    /// <param name="cancellationToken"></param>
    /// <returns>custom response with message</returns>
    [HttpPost("create")]
    [Authorize(Roles = "SuperAdministrator")]
    [ProducesResponseType(typeof(PharmacyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Result<PharmacyResponse>> CreatePharmacy([FromBody] CreatePharmacyRequest request,
        CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new CreatePharmacyCommand(request), cancellationToken);
        return response;
    }

    //upload logo
    /// <summary>
    ///     Upload logo
    /// </summary>
    /// <param name="id">pharmacy identifier</param>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>custom response with message</returns>
    [HttpPut("upload-logo/{id}")]
    [Authorize(Roles = "SuperAdministrator,SystemAdministrator")]
    [ProducesResponseType(typeof(PharmacyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Result<bool>> UploadLogo(long id, IFormFile file,
        CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new UploadPharmacyLogoCommand(id, file), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Get all pharmacies
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>list of all pharmacies</returns>
    [HttpGet("get-all-pharmacies")]
    [Authorize(Roles = "SuperAdministrator,SystemAdministrator")]
    public async Task<Result<List<PharmacyResponse>>> GetAllPharmacies(CancellationToken cancellationToken)
    {
        var
            response = await Sender.Send(new GetAllPharmaciesCommand(), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Get active pharmacies
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>list of active pharmacies</returns>
    [HttpGet("get-active-pharmacies")]
    public async Task<Result<List<PharmacyResponse>>> GetActivePharmacies(CancellationToken cancellationToken)
    {
        var
            response = await Sender.Send(new GetActivePharmaciesCommand(), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Get current user's pharmacy
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>pharmacy details for the current user</returns>
    [HttpGet("my-pharmacy")]
    [Authorize(Roles = "Pharmacist,SystemAdministrator")]
    [ProducesResponseType(typeof(PharmacyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Result<PharmacyResponse>> GetMyPharmacy(CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetMyPharmacyQuery(), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Get pharmacy by id
    /// </summary>
    /// <param name="id">pharmacy identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns>pharmacy details</returns>
    [HttpGet("get-pharmacy/{id}")]
    public async Task<Result<PharmacyResponse>> GetPharmacyById(long id, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetByIdPharmacyCommand(id), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Update pharmacy
    /// </summary>
    /// <param name="request">the updated details for the pharmacy</param>
    /// <param name="cancellationToken"></param>
    /// <returns>updated pharmacy details</returns>
    [HttpPut("update-pharmacy")]
    [Authorize(Roles = "SuperAdministrator,SystemAdministrator")]
    [ProducesResponseType(typeof(PharmacyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Result<PharmacyResponse>> UpdatePharmacy([FromBody] UpdatePharmacyRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePharmacyCommand(
            request.Id,
            request.Name,
            request.Description,
            request.ContactNumber,
            request.ContactEmail,
            request.ContactPerson,
            request.Address,
            request.OpeningTime,
            request.ClosingTime);

        var response = await Sender.Send(command, cancellationToken);
        return response;
    }

    /// <summary>
    ///     Toggle pharmacy active status
    /// </summary>
    /// <param name="id">pharmacy identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns>updated pharmacy details</returns>
    [HttpPut("toggle-active/{id}")]
    [Authorize(Roles = "SuperAdministrator")]
    [ProducesResponseType(typeof(PharmacyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Result<PharmacyResponse>> TogglePharmacyActive(long id, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new TogglePharmacyActiveCommand(id), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Delete pharmacy
    /// </summary>
    /// <param name="id">pharmacy identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns>custom response with message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdministrator")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Result<bool>> DeletePharmacy(long id, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new DeletePharmacyCommand(id), cancellationToken);
        return response;
    }
}