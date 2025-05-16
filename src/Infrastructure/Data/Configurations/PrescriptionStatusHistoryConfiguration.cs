using HelloDoctorApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelloDoctorApi.Infrastructure.Data.Configurations;

public class PrescriptionStatusHistoryConfiguration : IEntityTypeConfiguration<PrescriptionStatusHistory>
{
    public void Configure(EntityTypeBuilder<PrescriptionStatusHistory> builder)
    {
        builder.HasOne(p => p.ChangedByUser)
            .WithOne()
            .HasForeignKey<PrescriptionStatusHistory>(p => p.ChangedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Prescription)
            .WithMany(p => p.StatusHistory)
            .HasForeignKey(p => p.PrescriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(o => o.OldStatus)
            .HasConversion<string>();

        builder
            .Property(o => o.NewStatus)
            .HasConversion<string>();
    }
}