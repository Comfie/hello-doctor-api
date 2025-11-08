using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using HelloDoctorApi.Application.Prescriptions.Commands.AssignPrescriptionToPharmacy;
using HelloDoctorApi.Application.Prescriptions.Commands.CompleteDispense;
using HelloDoctorApi.Application.Prescriptions.Commands.MarkDelivered;
using HelloDoctorApi.Application.Prescriptions.Commands.UploadPrescription;
using HelloDoctorApi.Application.Prescriptions.Queries.GetPharmacyPrescriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Prescriptions.Models;
using HelloDoctorApi.Application.Prescriptions.Queries.GetPrescriptionDetails;
using HelloDoctorApi.Application.Prescriptions.Queries.ListAllPrescriptions;
using HelloDoctorApi.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HelloDoctorApi.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[TranslateResultToActionResult]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PrescriptionController : ApiController
{
    public PrescriptionController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    ///     Upload a prescription
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "MainMember")]
    [HttpPost("upload")] // member uploads a prescription for a beneficiary
    public async Task<Result<long>> Upload([FromBody] UploadPrescriptionCommand request,
        CancellationToken cancellationToken)
        => await Sender.Send(request, cancellationToken);

    // multipart/form-data upload with file
    // multipart/form-data upload with file
    [Authorize(Roles = "MainMember")]
    [HttpPost("upload-file")]
    [RequestSizeLimit(25_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<Result<long>> UploadFile([FromForm] UploadPrescriptionDto form,
        CancellationToken cancellationToken)
    {
        var service = new Services.UploadPrescriptionService(
            HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>(),
            HttpContext.RequestServices.GetRequiredService<IUser>(),
            HttpContext.RequestServices.GetRequiredService<IDocumentService>(),
            Sender
        );

        return await service.UploadAsync(form, cancellationToken);
    }

    /// <summary>
    ///     Assign a prescription to a pharmacy
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pharmacyId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "SystemAdministrator,SuperAdministrator,Pharmacist")]
    [HttpPost("{id:long}/assign/{pharmacyId:long}")]
    public async Task<Result<bool>> Assign(long id, long pharmacyId, CancellationToken cancellationToken)
        => await Sender.Send(new AssignPrescriptionToPharmacyCommand(id, pharmacyId), cancellationToken);

    /// <summary>
    ///     Complete dispense of a prescription
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>

    // Admin/system list all prescriptions (simple paging)
    [Authorize(Roles = "SystemAdministrator,SuperAdministrator,Pharmacist")]
    [HttpGet]
    public async
        Task<Result<List<ListPrescriptionItem>>>
        ListAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        => await Sender.Send(
            new ListAllPrescriptionsQuery(page,
                pageSize), ct);

    // Get prescription details
    [Authorize(Roles = "SystemAdministrator,SuperAdministrator,Pharmacist,MainMember")]
    [HttpGet("{id:long}")]
    public async
        Task<Result<PrescriptionDetailsDto>>
        GetById(long id, CancellationToken ct)
        => await Sender.Send(
            new GetPrescriptionDetailsQuery(id),
            ct);

    // Download prescription file
    [Authorize(Roles = "SystemAdministrator,SuperAdministrator,Pharmacist,MainMember")]
    [HttpGet("{id:long}/download/{fileId:long}")]
    public async Task<IActionResult> Download(long id, long fileId, [FromServices] IDocumentService docs,
        CancellationToken ct)
    {
        var bytes = await docs.GetPrescriptionFile(fileId, ct);
        if (!bytes.IsSuccess) return NotFound();
        // naive content type; adjust by file name if stored
        return File(bytes.Value, "application/pdf", fileDownloadName: $"prescription-{id}.pdf");
    }

    [Authorize(Roles = "SystemAdministrator,SuperAdministrator,Pharmacist")]
    [HttpPost("{id:long}/complete-dispense")]
    public async Task<Result<bool>> CompleteDispense(long id, [FromBody] CompleteDispenseDto dto,
        CancellationToken cancellationToken)
        => await Sender.Send(new CompleteDispenseCommand(id, dto.IsPartial, dto.Note), cancellationToken);

    /// <summary>
    ///     Deliver a prescription
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "SystemAdministrator,SuperAdministrator,Pharmacist")]
    [HttpPost("{id:long}/deliver")]
    public async Task<Result<bool>> Deliver(long id, CancellationToken cancellationToken)
        => await Sender.Send(new MarkDeliveredCommand(id), cancellationToken);

    /// <summary>
    ///     Get prescriptions for a pharmacy
    /// </summary>
    /// <param name="pharmacyId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "SystemAdministrator,SuperAdministrator,Pharmacist")]
    [HttpGet("pharmacy/{pharmacyId:long}")]
    public async Task<Result<List<GetPharmacyPrescriptionsResult>>> GetForPharmacy(long pharmacyId,
        CancellationToken cancellationToken)
        => await Sender.Send(new GetPharmacyPrescriptionsQuery(pharmacyId), cancellationToken);
}