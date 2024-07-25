using ApiBaseTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiBaseTemplate.Infrastructure.Data.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasOne(accountAdmin => accountAdmin.Account)
            .WithOne()
            .HasForeignKey<Doctor>(accountAdmin => accountAdmin.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(b => b.QualificationDescription)
            .HasMaxLength(100);
    }
}