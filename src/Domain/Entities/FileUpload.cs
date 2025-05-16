namespace HelloDoctorApi.Domain.Entities;

public class FileUpload : BaseAuditableEntity, ISoftDelete
{
    public required string FileName { get; set; }
    public required string Provider { get; set; }
    public required string Checksum { get; set; }
    public required byte[] FileContent { get; set; } //will use this in the meantime
    public required string Path { get; set; } //will use this when going live
    public DateTimeOffset UploadedDate { get; set; }
    public FileType FileType { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}