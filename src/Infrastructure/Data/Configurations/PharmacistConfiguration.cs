using ApiBaseTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiBaseTemplate.Infrastructure.Data.Configurations;

public class PharmacistConfiguration : IEntityTypeConfiguration<Pharmacist>
{
    public void Configure(EntityTypeBuilder<Pharmacist> builder)
    {
        builder
            .HasOne(pharmacist => pharmacist.Account)
            .WithOne()
            .HasForeignKey<Pharmacist>(pharmacist => pharmacist.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(pharmacist => pharmacist.Pharmacy)
            .WithOne()
            .HasForeignKey<Pharmacist>(pharmacist => pharmacist.PharmacyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}