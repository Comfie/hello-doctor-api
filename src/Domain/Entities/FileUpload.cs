namespace HelloDoctorApi.Domain.Entities;

public class FileUpload : BaseAuditableEntity, ISoftDelete
{
    public required string FileName { get; set; }
    public required string Provider { get; set; }
    public required string Checksum { get; set; }
    public required string Path { get; set; }
    public long FileSize { get; set; } // Track file size for analytics
    public DateTimeOffset UploadedDate { get; set; }
    public FileType FileType { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}