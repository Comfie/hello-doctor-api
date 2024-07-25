using ApiBaseTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiBaseTemplate.Infrastructure.Data.Configurations;

public class BenefactorConfiguration : IEntityTypeConfiguration<Benefactor>
{
    public void Configure(EntityTypeBuilder<Benefactor> builder)
    {
        
        builder.HasOne(benefactor => benefactor.Account)
            .WithOne()
            .HasForeignKey<Benefactor>(benefactor => benefactor.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Beneficiaries)
            .WithOne()
            .HasForeignKey(benefactor => benefactor.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}