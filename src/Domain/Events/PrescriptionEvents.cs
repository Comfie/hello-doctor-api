using HelloDoctorApi.Domain.Common;
using HelloDoctorApi.Domain.Enums;

namespace HelloDoctorApi.Domain.Events;

public sealed class PrescriptionAssignedToPharmacyEvent : BaseEvent
{
    public long PrescriptionId { get; }
    public long PharmacyId { get; }
    public PrescriptionStatus OldStatus { get; }
    public PrescriptionStatus NewStatus { get; }
    public PrescriptionAssignedToPharmacyEvent(long prescriptionId, long pharmacyId, PrescriptionStatus oldStatus, PrescriptionStatus newStatus)
    {
        PrescriptionId = prescriptionId;
        PharmacyId = pharmacyId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}

public sealed class PrescriptionDispensedEvent : BaseEvent
{
    public long PrescriptionId { get; }
    public bool IsPartial { get; }
    public string? Note { get; }
    public PrescriptionStatus OldStatus { get; }
    public PrescriptionStatus NewStatus { get; }
    public PrescriptionDispensedEvent(long prescriptionId, bool isPartial, string? note, PrescriptionStatus oldStatus, PrescriptionStatus newStatus)
    {
        PrescriptionId = prescriptionId;
        IsPartial = isPartial;
        Note = note;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}

public sealed class PrescriptionDeliveredEvent : BaseEvent
{
    public long PrescriptionId { get; }
    public PrescriptionStatus OldStatus { get; }
    public PrescriptionStatus NewStatus { get; }
    public PrescriptionDeliveredEvent(long prescriptionId, PrescriptionStatus oldStatus, PrescriptionStatus newStatus)
    {
        PrescriptionId = prescriptionId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}

