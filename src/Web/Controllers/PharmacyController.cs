using ApiBaseTemplate.Application.Pharmacies.Commands.CreatePharmacy;
using ApiBaseTemplate.Application.Pharmacies.Commands.UploadPharmacyLogo;
using ApiBaseTemplate.Application.Pharmacies.Models;
using ApiBaseTemplate.Application.Pharmacies.Queries.GetActivePharmacies;
using ApiBaseTemplate.Application.Pharmacies.Queries.GetAllPharmacies;
using ApiBaseTemplate.Application.Pharmacies.Queries.GetByIdPharmacy;
using ApiBaseTemplate.Application.Pharmacies.Updates.DeletePharmacy;
using ApiBaseTemplate.Domain.Shared;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ApiBaseTemplate.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class PharmacyController : ApiController
{
    public PharmacyController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Creates a new pharmacy
    /// </summary>
    /// <param name="request">the details for the pharmacy</param>
    /// <param name="cancellationToken"></param>
    /// <returns>custom response with message</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(PharmacyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePharmacy([FromBody] CreatePharmacyRequest request,
        CancellationToken cancellationToken)
    {
        Result<PharmacyResponse> response = await Sender.Send(new CreatePharmacyCommand(request), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    //upload logo
    /// <summary>
    /// Upload logo
    /// </summary>
    /// <param name="id">pharmacy identifier</param>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>custom response with message</returns>
    [HttpPut("upload-logo/{id}")]
    [ProducesResponseType(typeof(PharmacyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadLogo(long id, IFormFile file,
        CancellationToken cancellationToken)
    {
        Result<bool> response = await Sender.Send(new UploadPharmacyLogoCommand(id, file), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    /// <summary>
    /// Get all beneficiary
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>list of all pharmacies</returns>
    [HttpGet("get-all-pharmacies")]
    public async Task<IActionResult> GetAllPharmacies(CancellationToken cancellationToken)
    {
        Result<List<PharmacyResponse>>
            response = await Sender.Send(new GetAllPharmaciesCommand(), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    /// <summary>
    /// Get active pharmacies
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>list of active pharmacies</returns>
    [HttpGet("get-active-pharmacies")]
    public async Task<IActionResult> GetActivePharmacies(CancellationToken cancellationToken)
    {
        Result<List<PharmacyResponse>>
            response = await Sender.Send(new GetActivePharmaciesCommand(), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    /// <summary>
    /// Get pharmacy by id
    /// </summary>
    /// <param name="id">pharmacy identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns>pharmacy details</returns>
    [HttpGet("get-pharmacy/{id}")]
    public async Task<IActionResult> GetPharmacyById(long id, CancellationToken cancellationToken)
    {
        Result<PharmacyResponse> response = await Sender.Send(new GetByIdPharmacyCommand(id), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }


    /// <summary>
    /// Delete pharmacy
    /// </summary>
    /// <param name="id">pharmacy identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns>custom response with message</returns>
    [HttpPut("delete-pharmacy/{id}")]
    [ProducesResponseType(typeof(PharmacyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePharmacy(long id, CancellationToken cancellationToken)
    {
        Result<bool> response = await Sender.Send(new DeletePharmacyCommand(id), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
}