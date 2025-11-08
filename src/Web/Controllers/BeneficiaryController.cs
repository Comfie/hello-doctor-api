using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using HelloDoctorApi.Application.Beneficiaries.Commands.CreateBeneficiary;
using HelloDoctorApi.Application.Beneficiaries.Models;
using HelloDoctorApi.Application.Beneficiaries.Queries.GetBeneficiaries;
using HelloDoctorApi.Application.Beneficiaries.Queries.GetBeneficiariesByBenefactorId;
using HelloDoctorApi.Application.Beneficiaries.Queries.GetBeneficiary;
using HelloDoctorApi.Application.Beneficiaries.Updates.DeleteBeneficiary;
using HelloDoctorApi.Application.Beneficiaries.Updates.UpdateBeneficiary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoctorApi.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[TranslateResultToActionResult]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "MainMember")]
public class BeneficiaryController : ApiController
{
    public BeneficiaryController(ISender sender)
        : base(sender)
    {
    }

    /// <summary>
    ///     Creates a new beneficiary
    /// </summary>
    /// <param name="request">the deatils for the beneficiary</param>
    /// <param name="cancellationToken"></param>
    /// <returns>custom response with message</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(BeneficiaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Result<long>> CreateBeneficiary(
        [FromBody] CreateBeneficiaryRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await Sender.Send(new CreateBeneficiaryCommand(request), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Get all beneficiary
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>list of beneficiary</returns>
    [HttpGet("get-all-beneficiaries")]
    public async Task<Result<List<BeneficiaryResponse>>> GetAllBeneficiaries(
        CancellationToken cancellationToken = default)
    {
        var
            response = await Sender.Send(new GetBeneficiariesCommand(), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Get all beneficiaries for member
    /// </summary>
    /// <param name="id">main member identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns>list of beneficiary</returns>
    [HttpGet("get-all-member-beneficiaries/{id}")]
    public async Task<Result<List<BeneficiaryResponse>>> GetAllMemberBeneficiaries(string id,
        CancellationToken cancellationToken)
    {
        var
            response = await Sender.Send(new GetBeneficiariesByMainMemberIdCommand(id), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Get beneficiary by id
    /// </summary>
    /// <param name="id">beneficiary identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns>beneficiary details</returns>
    [HttpGet("get-beneficiary/{id}")]
    public async Task<Result<BeneficiaryResponse>> GetBeneficiaryById(long id, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetBeneficiaryCommand(id), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Update beneficiary
    /// </summary>
    /// <param name="request">the details for the beneficiary</param>
    /// <param name="cancellationToken"></param>
    /// <returns>custom response with message</returns>
    [HttpPut("update-beneficiary")]
    [ProducesResponseType(typeof(BeneficiaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Result<BeneficiaryResponse>> UpdateBeneficiary(
        [FromBody] UpdateBeneficiaryRequest request,
        CancellationToken cancellationToken)
    {
        var command =
            new UpdateBeneficiaryCommand(request.Id,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.EmailAddress,
                request.Gender,
                request.DateOfBirth,
                request.RelationshipToMainMember);

        var response =
            await Sender.Send(command, cancellationToken);
        return response;
    }

    //delete beneficiary
    /// <summary>
    ///     Delete beneficiary
    /// </summary>
    /// <param name="id">beneficiary identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns>custom response with message</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Result<bool>> DeleteBeneficiary(long id, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new DeleteBeneficiaryCommand(id), cancellationToken);
        return response;
    }
}