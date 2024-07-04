using ApiBaseTemplate.Application.Beneficiaries.Commands.CreateBeneficiary;
using ApiBaseTemplate.Application.Beneficiaries.Commands.GetBeneficiaries;
using ApiBaseTemplate.Application.Beneficiaries.Commands.GetBeneficiary;
using ApiBaseTemplate.Application.Beneficiaries.Models;
using ApiBaseTemplate.Domain.Shared;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ApiBaseTemplate.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class BeneficiaryController : ApiController
{
    public BeneficiaryController(ISender sender)
        : base(sender)
    {
    }

    //create
    /// <summary>
    /// Creates a new beneficiary
    /// </summary>
    /// <param name="request">the deatils for the beneficiary</param>
    /// <param name="cancellationToken"></param>
    /// <returns>custom response with message</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(BeneficiaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateBeneficiary(
        [FromBody] CreateBeneficiaryRequest request,
        CancellationToken cancellationToken)
    {
        Result<long> response = await Sender.Send(new CreateBeneficiaryCommand(request), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    /// <summary>
    /// Get all beneficiary
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>list of beneficiary</returns>
    [HttpGet("get-all-beneficiaries")]
    public async Task<IActionResult> GetAllBeneficiaries(CancellationToken cancellationToken)
    {
        Result<List<BeneficiaryResponse>>
            response = await Sender.Send(new GetBeneficiariesCommand(), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    /// <summary>
    /// Get beneficiary by id
    /// </summary>
    /// <param name="id">beneficiary identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns>beneficiary details</returns>
    [HttpGet("get-beneficiary/{id}")]
    public async Task<IActionResult> GetBeneficiaryById(long id, CancellationToken cancellationToken)
    {
        Result<BeneficiaryResponse> response = await Sender.Send(new GetBeneficiaryCommand(id), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
}