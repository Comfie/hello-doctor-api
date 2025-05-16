using HelloDoctorApi.Domain.Entities.Auth;

namespace HelloDoctorApi.Domain.Entities;

public class PrescriptionNote
{
    public long Id { get; set; }
    public long PrescriptionId { get; set; }
    public required Prescription Prescription { get; set; }
    public required string UserId { get; set; } // ID of the user who added the note
    public required ApplicationUser User { get; set; }
    public required UserType UserType { get; set; } // "Doctor", "Pharmacist", etc.
    public required string Note { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public bool IsPrivate { get; set; } // If true, only visible to healthcare providers
    public bool IsSystemGenerated { get; set; } // For automated system notes
    public string? ReferencedItemId { get; set; } // Optional reference to specific prescription item
}