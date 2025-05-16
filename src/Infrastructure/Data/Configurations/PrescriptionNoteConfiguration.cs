using HelloDoctorApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelloDoctorApi.Infrastructure.Data.Configurations;

public class PrescriptionNoteConfiguration : IEntityTypeConfiguration<PrescriptionNote>
{
    public void Configure(EntityTypeBuilder<PrescriptionNote> builder)
    {
        builder.HasOne(p => p.Prescription)
            .WithMany(p => p.PrescriptionNotes)
            .HasForeignKey(p => p.PrescriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<PrescriptionNote>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(o => o.UserType)
            .HasConversion<string>();
    }
}