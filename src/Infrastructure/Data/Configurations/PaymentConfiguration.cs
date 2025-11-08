using HelloDoctorApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelloDoctorApi.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Configure relationship with Payer (ApplicationUser)
        builder
            .HasOne(p => p.Payer)
            .WithMany()
            .HasForeignKey(p => p.PayerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship with Prescription (optional)
        builder
            .HasOne(p => p.Prescription)
            .WithMany()
            .HasForeignKey(p => p.PrescriptionId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Configure relationship with Invoice (optional)
        builder
            .HasOne(p => p.Invoice)
            .WithMany()
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Configure decimal precision for Amount
        builder
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        // Configure enum conversions to string for better readability in database
        builder
            .Property(p => p.Status)
            .HasConversion<string>();

        builder
            .Property(p => p.Purpose)
            .HasConversion<string>();

        builder
            .Property(p => p.Method)
            .HasConversion<string>();

        builder
            .Property(p => p.Provider)
            .HasConversion<string>();

        // Configure string lengths
        builder
            .Property(p => p.Currency)
            .HasMaxLength(3); // ISO 4217 currency codes are 3 characters

        builder
            .Property(p => p.PayeeType)
            .HasMaxLength(50);

        // Configure indexes for better query performance
        builder
            .HasIndex(p => p.PayerId);

        builder
            .HasIndex(p => p.Status);

        builder
            .HasIndex(p => p.PrescriptionId);

        builder
            .HasIndex(p => p.ExternalTransactionId);
    }
}
