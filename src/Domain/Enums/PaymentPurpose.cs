namespace HelloDoctorApi.Domain.Enums;

/// <summary>
/// Defines the purpose/reason for a payment
/// </summary>
public enum PaymentPurpose
{
    /// <summary>
    /// Payment for doctor consultation/prescription fee
    /// </summary>
    PrescriptionFee = 0,

    /// <summary>
    /// Payment to pharmacy for dispensing medicine
    /// </summary>
    DispenseFee = 1,

    /// <summary>
    /// Payment for prescription delivery
    /// </summary>
    DeliveryFee = 2,

    /// <summary>
    /// Refund for cancelled prescription
    /// </summary>
    Refund = 3
}
