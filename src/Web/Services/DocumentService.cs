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

        // Reset stream position for checksum calculation
        file.Position = 0;
        var checksum = await _checksumService.ChecksumAsHexAsync(file, cancellationToken);

        // Get file size
        var fileSize = file.Length;

        var fileInfo = new FileUpload
        {
            FileName = fileName,
            Path = path,
            UploadedDate = _dateTime.OffsetNow,
            Provider = _fileStoreService.GetType().Name, // Track actual provider
            FileType = FileType.Prescription,
            Checksum = checksum,
            FileSize = fileSize
        };
        _context.FileUploads.Add(fileInfo);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(fileInfo.Id);
    }

    public async Task<Result<byte[]>> GetPrescriptionFile(long id, CancellationToken cancellationToken = default)
    {
        var file = await _context.FileUploads
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (file is null)
            return Result<byte[]>.Error(new Error("Get Prescription File", "File not found"));

        try
        {
            var fileContent = await _fileStoreService.GetFile(file.Path, cancellationToken);
            return Result<byte[]>.Success(fileContent);
        }
        catch (Exception ex)
        {
            return Result<byte[]>.Error(new Error("Get Prescription File", $"Failed to retrieve file: {ex.Message}"));
        }
    }

    public async Task<Result<bool>> DeletePrescriptionFile(long id, CancellationToken cancellationToken = default)
    {
        var file = await _context.FileUploads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (file is null)
            return Result<bool>.Error(new Error("Delete Prescription File", "File not found"));

        // Soft delete in database
        file.IsDeleted = true;
        file.DeletedAt = _dateTime.OffsetNow;
        _context.FileUploads.Update(file);
        await _context.SaveChangesAsync(cancellationToken);

        // Optionally delete from storage asynchronously (background job recommended)
        // For now, we keep the file in storage for potential recovery

        return Result<bool>.Success(true);
    }
}