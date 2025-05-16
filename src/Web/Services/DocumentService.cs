using Ardalis.Result;
using HelloDoctorApi.Application.Common.Interfaces;
using HelloDoctorApi.Domain.Entities;
using HelloDoctorApi.Domain.Enums;
using HelloDoctorApi.Domain.Shared;
using HelloDoctorApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelloDoctorApi.Web.Services;

public class DocumentService : IDocumentService
{
    private readonly IChecksumService _checksumService;
    private readonly ApplicationDbContext _context;
    private readonly IFileStoreService _fileStoreService;
    private readonly IDateTimeService _dateTime;

    public DocumentService(ApplicationDbContext context,
        IFileStoreService fileStoreService,
        IChecksumService checksumService,
        IDateTimeService dateTime)
    {
        _context = context;
        _fileStoreService = fileStoreService;
        _checksumService = checksumService;
        _dateTime = dateTime;
    }

    public async Task<Result<long>> SavePrescriptionFile(Stream file, long mainMemberId,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("File is empty or null");

        var member = await _context.MainMembers.FirstOrDefaultAsync(x => x.Id == mainMemberId, cancellationToken);

        if (member is null)
            return Result<long>.Error(new Error("Save Prescription File", "MainMember not found"));

        var fileName = $"Prescription_{member.Code}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

        var uuid = Guid.NewGuid();
        var path = await _fileStoreService.GeneratePath(uuid, fileName, cancellationToken);

        try
        {
            await _fileStoreService.SaveFile(file, path, cancellationToken);
        }
        catch (Exception e)
        {
            return Result<long>.Error(e.Message);
        }

        var checksum = await _checksumService.ChecksumAsHexAsync(file, cancellationToken);

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        var fileInfo = new FileUpload
        {
            FileName = fileName,
            Path = path,
            UploadedDate = _dateTime.OffsetNow,
            Provider = "Local",
            FileType = FileType.Prescription,
            Checksum = checksum,
            FileContent = memoryStream.ToArray(),
        };
        _context.FileUploads.Add(fileInfo);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(fileInfo.Id);
    }

    public async Task<Result<byte[]>> GetPrescriptionFile(long id, CancellationToken cancellationToken = default)
    {
        var file = await _context.FileUploads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (file is null)
            return Result<byte[]>.Error(new Error("Get Prescription File", "File not found"));

        return Result<byte[]>.Success(file.FileContent);
    }

    public async Task<Result<bool>> DeletePrescriptionFile(long id, CancellationToken cancellationToken = default)
    {
        var file = await _context.FileUploads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (file is null)
            return Result<bool>.Error(new Error("Delete Prescription File", "File not found"));

        file.IsDeleted = true;
        _context.FileUploads.Update(file);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}