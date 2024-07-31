using ApiBaseTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiBaseTemplate.Infrastructure.Data.Configurations;

public class BeneficiaryConfiguration : IEntityTypeConfiguration<Beneficiary>
{
    public void Configure(EntityTypeBuilder<Beneficiary> builder)
    {
        builder
            .HasOne(o => o.MainMember)
            .WithOne()
            .HasForeignKey<Beneficiary>(o => o.MainMemberId)
            .OnDelete(DeleteBehavior.Cascade);
        
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
            .Property(b => b.PhoneNumber)
            .HasMaxLength(100)
            .IsRequired();
        
        builder
            .HasIndex(b => b.EmailAddress)
            .IsUnique();
        
        builder
            .HasIndex(b => b.PhoneNumber)
            .IsUnique();
        
        builder
            .HasIndex(b => b.FirstName);
        
        builder
            .HasIndex(b => b.LastName);
        
        builder
            .Property(o => o.Relationship)
            .HasConversion<string>();
        
    }
}