using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Application.Prescriptions.Commands.CreatePrescriptionFromUpload;
using HelloDoctorApi.Application.Prescriptions.Models;
using HelloDoctorApi.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Web.Services;

public class UploadPrescriptionService : IUploadPrescriptionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUser _user;
    private readonly IDocumentService _documentService;
    private readonly ISender _sender;

    public UploadPrescriptionService(ApplicationDbContext dbContext, IUser user,
        IDocumentService documentService, ISender sender)
    {
        _dbContext = dbContext;
        _user = user;
        _documentService = documentService;
        _sender = sender;
    }

    public async Task<Result<long>> UploadAsync(UploadPrescriptionDto form, CancellationToken cancellationToken)
    {
        if (form.File is null || form.File.Length == 0)
        {
            return Result<long>.Invalid(new[] { new ValidationError { ErrorMessage = "File is required" } });
        }

        await using var stream = form.File.OpenReadStream();

        var member = await _dbContext.MainMembers.AsNoTracking()
            .FirstOrDefaultAsync(m => m.AccountId == _user.Id, cancellationToken);
        if (member is null) return Result<long>.Unauthorized();

        var saveRes = await _documentService.SavePrescriptionFile(stream, member.Id, cancellationToken);
        if (!saveRes.IsSuccess)
            return Result<long>.Error(saveRes.Errors.FirstOrDefault() ?? "Failed to save file");

        var created = await _sender
            .Send(
                new CreatePrescriptionFromUploadCommand(form.BeneficiaryId, saveRes.Value, form.PharmacyId, form.Notes),
                cancellationToken);

        return created;
    }
}