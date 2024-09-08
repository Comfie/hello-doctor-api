using HelloDoctorApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelloDoctorApi.Infrastructure.Data.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder
            .HasOne(p => p.MainMember)
            .WithOne()
            .HasForeignKey<Prescription>(p => p.MainMemberId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(p => p.Beneficiary)
            .WithOne()
            .HasForeignKey<Prescription>(p => p.BeneficiaryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}