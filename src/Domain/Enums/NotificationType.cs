namespace HelloDoctorApi.Domain.Enums;

public enum NotificationType
{
    PrescriptionUploaded = 0,
    PrescriptionAssigned = 1,
    PrescriptionDispensed = 2,
    PrescriptionPartiallyDispensed = 3,
    PrescriptionDelivered = 4,
    PrescriptionExpired = 5,
    PaymentReceived = 6,
    PaymentFailed = 7,
    InvoiceGenerated = 8,
    LowStockAlert = 9,
    SystemAnnouncement = 10,
    MessageReceived = 11
}
