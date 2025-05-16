using HelloDoctorApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelloDoctorApi.Infrastructure.Data.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder
            .Property(b => b.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(b => b.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(b => b.EmailAddress)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(b => b.PrimaryContact)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .HasIndex(b => b.EmailAddress)
            .IsUnique();

        builder
            .HasIndex(b => b.PrimaryContact)
            .IsUnique();
        builder
            .Property(b => b.QualificationDescription)
            .HasMaxLength(100);

        builder.HasMany(d => d.Pharmacies)
            .WithMany(p => p.Doctors);
    }
}