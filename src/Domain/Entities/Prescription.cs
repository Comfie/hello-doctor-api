using HelloDoctorApi.Domain.Common;
using HelloDoctorApi.Domain.Entities.Auth;
using HelloDoctorApi.Domain.Enums;
using HelloDoctorApi.Domain.Events;

namespace HelloDoctorApi.Domain.Entities;

public class Prescription : BaseAuditableEntity
{
    public string? Notes { get; set; }
    public DateTimeOffset IssuedDate { get; set; }
    public DateTimeOffset ExpiryDate { get; set; }
    public PrescriptionStatus Status { get; set; }

    // Assignment
    public long? AssignedPharmacyId { get; private set; }
    public Pharmacy? AssignedPharmacy { get; private set; }

    // Ownership
    public required string MainMemberId { get; set; }
    public required ApplicationUser MainMember { get; set; }
    public long BeneficiaryId { get; set; }
    public required Beneficiary Beneficiary { get; set; }

    // Doctor (optional - prescriptions can be uploaded by MainMembers)
    public long? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    // Files & Notes
    public List<FileUpload>? PrescriptionFiles { get; set; } = new();
    public List<PrescriptionNote>? PrescriptionNotes { get; set; } = new();
    public List<PrescriptionStatusHistory>? StatusHistory { get; set; } = new();

    // Behavior (minimal guards + domain events)
    public void AssignToPharmacy(long pharmacyId)
    {
        if (Status != PrescriptionStatus.Pending && Status != PrescriptionStatus.Approved && Status != PrescriptionStatus.PaymentPending && Status != PrescriptionStatus.ReadyForPickup)
            return;

        var old = Status;
        AssignedPharmacyId = pharmacyId;
        Status = PrescriptionStatus.UnderReview;
        AddDomainEvent(new Events.PrescriptionAssignedToPharmacyEvent(Id, pharmacyId, old, Status));
    }

    public void CompleteDispense(bool isPartial, string? note = null)
    {
        if (Status != PrescriptionStatus.UnderReview && Status != PrescriptionStatus.OnHold)
            return;

        var old = Status;
        Status = isPartial ? PrescriptionStatus.PartiallyDispensed : PrescriptionStatus.FullyDispensed;
        AddDomainEvent(new Events.PrescriptionDispensedEvent(Id, isPartial, note, old, Status));
    }

    public void MarkDelivered()
    {
        if (Status != PrescriptionStatus.FullyDispensed && Status != PrescriptionStatus.PartiallyDispensed && Status != PrescriptionStatus.ReadyForPickup)
            return;

        var old = Status;
        Status = PrescriptionStatus.Delivered;
        AddDomainEvent(new Events.PrescriptionDeliveredEvent(Id, old, Status));
    }

    public void MarkAsAwaitingPayment()
    {
        if (Status != PrescriptionStatus.Pending)
            return;

        Status = PrescriptionStatus.PaymentPending;
    }

    public void ApproveAfterPayment()
    {
        if (Status != PrescriptionStatus.PaymentPending)
            return;

        Status = PrescriptionStatus.Approved;
    }

    public void RejectDueToFailedPayment(string reason)
    {
        if (Status != PrescriptionStatus.PaymentPending)
            return;

        Status = PrescriptionStatus.OnHold;
        // Note about payment failure will be added via PrescriptionNote
    }
}
