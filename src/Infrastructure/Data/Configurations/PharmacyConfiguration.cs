using HelloDoctorApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelloDoctorApi.Infrastructure.Data.Configurations;

public class PharmacyConfiguration : IEntityTypeConfiguration<Pharmacy>
{
    public void Configure(EntityTypeBuilder<Pharmacy> builder)
    {
        builder
            .Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder
            .Property(p => p.Address)
            .HasMaxLength(300);
        
        builder
            .Property(p => p.Description)
            .HasMaxLength(300);
        
        builder
            .Property(p => p.ContactPerson)
            .HasMaxLength(100);
        
        builder
            .Property(p => p.ContactNumber)
            .HasMaxLength(25);
        
        builder
            .Property(p => p.ContactEmail)
            .HasMaxLength(25);
        
        builder
            .HasMany(p => p.Pharmacists)
            .WithOne(b => b.Pharmacy)
            .HasForeignKey(b => b.PharmacyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(p => p.IsActive)
            .HasDefaultValue(true);
        
        builder
            .Property(p => p.IsDeleted)
            .HasDefaultValue(false);
        
        builder
            .HasMany(p => p.Doctors)
            .WithMany(p => p.Pharmacies);
    }
}