namespace HelloDoctorApi.Domain.Enums;

public enum PrescriptionStatus
{
    Pending = 0, // Initially uploaded/created
    UnderReview = 1, // Pharmacist is reviewing
    Approved = 2, // Approved by pharmacist
    PartiallyDispensed = 3, // Some items dispensed
    FullyDispensed = 4, // All items dispensed
    OnHold = 5, // Requires clarification/out of stock
    Rejected = 6, // Rejected by pharmacist
    Expired = 7, // Prescription has expired
    Cancelled = 8, // Cancelled by doctor/patient
    PaymentPending = 9, // Waiting for payment
    ReadyForPickup = 10 // Prepared and ready for collection
}