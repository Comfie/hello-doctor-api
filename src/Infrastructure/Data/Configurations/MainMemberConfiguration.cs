using HelloDoctorApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelloDoctorApi.Infrastructure.Data.Configurations;

public class MainMemberConfiguration : IEntityTypeConfiguration<MainMember>
{
    public void Configure(EntityTypeBuilder<MainMember> builder)
    {
        
        builder.HasOne(mainMember => mainMember.Account)
            .WithOne(account => account.MainMember)
            .HasForeignKey<MainMember>(mainMember => mainMember.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Beneficiaries)
            .WithOne()
            .HasForeignKey(mainMember => mainMember.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}